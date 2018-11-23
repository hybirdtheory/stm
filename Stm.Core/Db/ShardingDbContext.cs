using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core.Db
{
    public abstract class ShardingDbContext:IDisposable
    {

        public class InnerDbcontext : DbContext
        {
            private ShardingDbContext _shardingDbContext;

            internal InnerDbcontext ( ShardingDbContext shardingDbContext,  DbContextOptions<InnerDbcontext> options ) : base( options )
            {
                _shardingDbContext = shardingDbContext;
            }


            protected override void OnModelCreating ( ModelBuilder modelBuilder )
            {
                _shardingDbContext.OnModelCreating( modelBuilder );
            }
        }


        internal class ShardingEntityComparer<T> : IEqualityComparer<T>
        {
            private ShardingDbContext _shardingDbContext;

            public ShardingEntityComparer ( ShardingDbContext shardingDbContext)
            {
                _shardingDbContext = shardingDbContext;
            }

            public bool Equals ( T x, T y )
            {
                return (long)_shardingDbContext.getPkProperty<T>().GetValue( x ) == (long)_shardingDbContext.getPkProperty<T>().GetValue( y );
            }

            public int GetHashCode ( T obj )
            {
                return _shardingDbContext.getPkProperty<T>().GetValue( obj ).GetHashCode();
            }
        }

        protected abstract void OnModelCreating ( ModelBuilder modelBuilder );

        /// <summary>
        /// 分库信息
        /// </summary>
        public List<DbShardingItem> ShardingInfos { get; private set; }

        /// <summary>
        /// 连接字符串和dbcontext的映射
        /// </summary>
        private Dictionary<String, InnerDbcontext> _activeDbcontexts;

        /// <summary>
        /// 类型对应的主键属性
        /// </summary>
        private Dictionary<Type, PropertyInfo> _typePks;

        /// <summary>
        /// 通过连接字符串获取数据配置信息的代理
        /// </summary>
        private Func<string, DbContextOptions<InnerDbcontext>> _innerDbContextOptionsDelegte;


        private object lockobj=new object();

        public ShardingDbContext( IShardingDbcontextOptions options )
            :this( options?.ShardingConnections?.GetDbShardingItems(),options.InnerDbContextOptionsDelegte)
        {

        }

        public ShardingDbContext ( Action<string, DbContextOptionsBuilder<InnerDbcontext>> innerDbcontextOptionsDelegte )
            : this(null, innerDbcontextOptionsDelegte )
        {
        }
        public ShardingDbContext ( List<DbShardingItem> shardingInfos, Action<string, DbContextOptionsBuilder<InnerDbcontext>> innerDbcontextOptionsDelegte )
        {
            ShardingInfos = shardingInfos??new List<DbShardingItem>();
            _activeDbcontexts = new Dictionary<String, InnerDbcontext>();
            _typePks = new Dictionary<Type, PropertyInfo>();
            _innerDbContextOptionsDelegte = connstring =>
            {
                var builder = new DbContextOptionsBuilder<InnerDbcontext>();
                innerDbcontextOptionsDelegte( connstring, builder );
                return builder.Options;
            };

            //查找主键
            var conventionSet = new ConventionSet();
            var modelBuilder = new ModelBuilder( conventionSet );
            OnModelCreating( modelBuilder );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                _typePks.Add( entityType.ClrType, entityType.FindPrimaryKey().Properties[0].PropertyInfo );
            }
        }

        private InnerDbcontext getDbContext ( DbInfo dbInfo )
        {
            return new InnerDbcontext(this, _innerDbContextOptionsDelegte(dbInfo.ConnectionString) );
        }

        private DbContext GetDbContext (DbInfo dbInfo )
        {
            if (!_activeDbcontexts.ContainsKey( dbInfo.ConnectionString ))
            {
                lock (lockobj)
                {
                    if (!_activeDbcontexts.ContainsKey( dbInfo.ConnectionString ))
                    {
                        var dbcontext = getDbContext( dbInfo );
                        _activeDbcontexts.Add( dbInfo.ConnectionString, dbcontext );
                    }
                }
            }
            return _activeDbcontexts[dbInfo.ConnectionString];
        }

        private PropertyInfo getPkProperty<T> ()
        {
            var property = _typePks[typeof( T )];

            if (property == null)
            {
                throw new Exception( $"type '{typeof(T).Name}' is not register into dbcontext" );
            }

            return property;
        }

        /// <summary>
        /// 根据键值获取所在的数据库
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public DbContext GetDbContextByKey(long keyvalue )
        {
            DbShardingItem dbShardingItem = null;

            foreach (var item in ShardingInfos)
            {
                if (keyvalue >= item.IdMin && keyvalue <= item.IdMax)
                {
                    dbShardingItem = item;
                    break;
                }
            }

            if (dbShardingItem == null)
            {
                throw new Exception( $" keyvalue '{keyvalue}' out of sharding range" );
            }

            //根据id求模，选出数据库
            var index = (int)(keyvalue % (dbShardingItem.Servers.Count()));

            var dbserver = dbShardingItem.Servers[index];

            DbContext dbContext = GetDbContext( dbserver );

            return dbContext;
        }

        public async Task AddAsync<T> ( T entity )
            where T : class
        {
            var id = (long)getPkProperty<T>().GetValue( entity );

            DbContext dbContext = GetDbContextByKey( id );

            await dbContext.AddAsync( entity );
        }

        public async Task<T> FindAsync<T> ( long id )
            where T:class
        {

            DbShardingItem dbShardingItem = null;

            foreach (var item in ShardingInfos)
            {
                if (id >= item.IdMin && id <= item.IdMax)
                {
                    dbShardingItem = item;
                    break;
                }
            }

            if (dbShardingItem == null)
            {
                throw new Exception( $" idvalue '{id}' out of sharding range" );
            }

            //hash模式
            if (dbShardingItem.ReadMode == ShardingReadMode.Hash)
            {

                //根据id求模，选出数据库
                var index = (int)(id % (dbShardingItem.Servers.Count()));

                var dbserver = dbShardingItem.Servers[index];

                DbContext dbContext = GetDbContext( dbserver );

                return await dbContext.FindAsync<T>( id);
            }
            //完全模式
            else
            {
                List<Task<T>> tasks = new List<Task<T>>();
                foreach(var dbserver in dbShardingItem.Servers)
                {
                    DbContext dbContext = GetDbContext( dbserver );

                    tasks.Add( dbContext.FindAsync<T>( id ) );
                }

                var result = await Task.WhenAll( tasks );

                return result.FirstOrDefault();
            }
        }

        public async Task SaveChanges ()
        {
            await Task.WhenAll( _activeDbcontexts.Select( t => t.Value.SaveChangesAsync() ) );
        }

        public void Remove<T>(T entity )
            where T:class
        {
            foreach(var dbcontext in _activeDbcontexts)
            {
                if (dbcontext.Value.ChangeTracker.Entries<T>().Any( t=>t.Entity==entity ))
                {
                    dbcontext.Value.Remove( entity );
                }
            }
        }

        /// <summary>
        /// 查询数量
        /// 暂不能保证扩容时返回正常值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wherePredicate"></param>
        /// <returns></returns>
        public async Task<long> CountAsync<T> ( Expression<Func<T,bool>> wherePredicate )
            where T:class
        {
            List<Task<int>> tasks = new List<Task<int>>();

            foreach (var sharding in ShardingInfos)
            {
                foreach(var dbserver in sharding.Servers)
                {
                    var dbContext = GetDbContext( dbserver );
                    tasks.Add( dbContext.Set<T>().CountAsync( wherePredicate ) );
                }
            }
            var result = await Task.WhenAll( tasks );

            return result.Select(t=>(long)t).DefaultIfEmpty(0).Sum();
        }

        /// <summary>
        /// 查询数量
        /// 暂不能保证扩容时返回正常值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<long> CountAsync<T> ( )
            where T:class
        {
            List<Task<int>> tasks = new List<Task<int>>();

            foreach (var sharding in ShardingInfos)
            {
                foreach (var dbserver in sharding.Servers)
                {
                    var dbContext = GetDbContext( dbserver );
                    tasks.Add( dbContext.Set<T>().CountAsync(  ) );
                }
            }
            var result = await Task.WhenAll( tasks );

            return result.Select( t => (long)t ).DefaultIfEmpty( 0 ).Sum();
        }

        public async Task<bool> AnyAsync<T> ()
            where T:class
        {
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (var sharding in ShardingInfos)
            {
                foreach (var dbserver in sharding.Servers)
                {
                    var dbContext = GetDbContext( dbserver );
                    tasks.Add( dbContext.Set<T>().AnyAsync() );
                }
            }
            var result = await Task.WhenAll( tasks );

            return result.DefaultIfEmpty( false ).Any();
        }

        public async Task<bool> AnyAsync<T> ( System.Linq.Expressions.Expression<Func<T, bool>> wherePredicate )
            where T:class
        {
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (var sharding in ShardingInfos)
            {
                foreach (var dbserver in sharding.Servers)
                {
                    var dbContext = GetDbContext( dbserver );
                    tasks.Add( dbContext.Set<T>().AnyAsync( wherePredicate ) );
                }
            }
            var result = await Task.WhenAll( tasks );

            return result.DefaultIfEmpty( false ).Any();
        }

        public async Task<List<T>> QueryAsync<T> ( System.Linq.Expressions.Expression<Func<T, bool>> wherePredicate )
            where T:class
        {
            List<Task<List<T>>> tasks = new List<Task<List<T>>>();

            foreach (var sharding in ShardingInfos)
            {
                foreach (var dbserver in sharding.Servers)
                {
                    var dbContext = GetDbContext( dbserver );
                    tasks.Add( dbContext.Set<T>().Where( wherePredicate ).ToListAsync() );
                }
            }
            var result = await Task.WhenAll( tasks );

            return result.SelectMany(t=>t).ToList().Distinct(new ShardingEntityComparer<T>(this)).ToList();
        }

        public async Task<List<T>> PageQueryAsync<T> ( 
            Expression<Func<T, bool>> wherePredicate, 
            int pagesize,
            string ordertype, 
            long lastid )
            where T:class
        {
            List<Task<List<T>>> tasks = new List<Task<List<T>>>();

            foreach (var sharding in ShardingInfos)
            {
                foreach (var dbserver in sharding.Servers)
                {
                    var dbContext = GetDbContext( dbserver );

                    var query = dbContext.Set<T>().Where( wherePredicate );

                    if (ordertype == "asc")
                    {
                        var exp = GetGreaterThanWherePredicate<T>(lastid);
                        var whereExp = (Expression<Func<T, bool>>)exp;
                        var orderExp = (Expression<Func<T, long>>)GetKeySelector<T>(  lastid );
                        query =query.OrderBy( orderExp ).Where( whereExp ).Take(pagesize);
                    }
                    else
                    {
                        var exp = GetLessThanWherePredicate<T>(lastid );
                        var whereExp = (Expression<Func<T, bool>>)exp;
                        var orderExp = (Expression<Func<T, long>>)GetKeySelector<T>(lastid );
                        query = query.OrderByDescending( orderExp ).Where( whereExp ).Take( pagesize );
                    }

                    tasks.Add( query.ToListAsync() );
                }
            }
            var taskResults = await Task.WhenAll( tasks );

            var allList = taskResults.SelectMany( t => t ).Distinct( new ShardingEntityComparer<T>( this ) );

            if (ordertype == "asc")
            {
                var result = allList.OrderBy( t => (long)getPkProperty<T>().GetValue( t ) );

                return result.Take( pagesize ).ToList();
            }
            else
            {

                var result = allList.OrderByDescending( t => (long)getPkProperty<T>().GetValue( t ) );

                return result.Take( pagesize ).ToList();
            }
        }

        private Expression GetGreaterThanWherePredicate<T> (long lastid)
            where T:class
        {

            ParameterExpression param = Expression.Parameter( typeof( T ), "t" );

            Expression body = Expression.Property( param, getPkProperty<T>() );

            Expression exp = Expression.GreaterThan( body, Expression.Constant( lastid ) );

            return Expression.Lambda( exp, param );
        }
        private Expression GetLessThanWherePredicate<T> (  long lastid )
            where T:class
        {

            ParameterExpression param = Expression.Parameter( typeof( T ), "t" );

            Expression body = Expression.Property( param, getPkProperty<T>() );

            Expression exp = Expression.LessThan( body, Expression.Constant( lastid ) );

            return Expression.Lambda( exp, param );
        }

        private Expression GetKeySelector<T> ( long lastid )
            where T:class
        {
            ParameterExpression param = Expression.Parameter( typeof( T ), "t" );
            Expression body = Expression.Property( param, getPkProperty<T>() );

            return Expression.Lambda( body, param );
        }

        public void Dispose ()
        {
            foreach (var item in this._activeDbcontexts)
            {
                try
                {
                    item.Value.Dispose();
                }
                catch
                {

                }
            }
            this._activeDbcontexts.Clear();

        }
    }

}
