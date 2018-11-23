using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stm.Core.Db;
using Stm.Core.Domain.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stm.Mvcdemo
{
    public class OrderShardingDbcontext : ShardingDbContext
    {
        public OrderShardingDbcontext(ShardingDbcontextOptions<OrderShardingDbcontext> options)
            : base( options )
        {

        }


        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {

            modelBuilder.Entity<OrderInfo>( entity =>
            {
                entity.ToTable( "Orders" );

                entity.HasKey( t => t.OrderId );

                entity.Property( t => t.OrderContent );
            } );
        }
    }

    public class CommonDb : DbContext
    {
        public CommonDb ( DbContextOptions<CommonDb> options ) : base( options )
        {

        }

        public CommonDb ( String connectionString )
            :base( new DbContextOptionsBuilder<CommonDb>().UseSqlServer( connectionString ).Options )
        {
        }


        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            modelBuilder.Entity<SysConfigInfo>( entity =>
            {
                entity.ToTable( "config" );

                entity.HasKey( t => t.Key );

                entity.Property( t => t.Value );
                entity.Property( t => t.LastValue );
                entity.Property( t => t.Version );
                entity.Property( t => t.EffectiveDt );
                entity.Property( t => t.IsDeleted );

            } );

            modelBuilder.Entity<AuditLogInfo>( entity =>
            {
                entity.ToTable( "AuditLog" );

                entity.HasKey( t => t.AuditLogId );

                entity.Property( t => t.LogDt );
                entity.Property( t => t.Content );
                entity.Property( t => t.UserId );
                entity.Property( t => t.UserName );
            } );
            modelBuilder.Entity<OrderInfo>( entity =>
            {
                entity.ToTable( "Orders" );

                entity.HasKey( t => t.OrderId );

                entity.Property( t => t.OrderContent );
            } );


            //modelBuilder.Entity<SafecodeInfo>( entity =>
            //{
            //    entity.ToTable( "safecode" );

            //    entity.HasKey( t => t.Token );

            //    entity.Property( t => t.Token );
            //    entity.Property( t => t.Code );
            //    entity.Property( t => t.ExpirDt );

            //} );

            //modelBuilder.Entity<User>( entity =>
            //{
            //    entity.ToTable( "adminuser" );

            //    entity.HasKey( t => t.Userid );

            //    entity.Property( t => t.Userid );
            //    entity.Property( t => t.Avatar );
            //    entity.Property( t => t.CreateDt );
            //    entity.Property( t => t.Name );
            //    entity.Property( t => t.Password );
            //    entity.Property( t => t.Phone );
            //    entity.Property( t => t.Salt );
            //    entity.Property( t => t.Username );

            //} );
        }
    }
}
