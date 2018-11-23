using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Stm.Core.Domain.Generic
{
    /// <summary>
    /// 资源描述
    /// </summary>
    public class ResourceDescriptor: IServiceDto
    {
        /// <summary>
        /// 过期时间
        /// 防止存储的token过多
        /// </summary>
        private DateTime _expireDt;

        /// <summary>
        /// 资源项
        /// </summary>
        private List<ResourceItemDescriptor> _resourceItemDescriptors;

        public ResourceDescriptor ()
        {
            _resourceItemDescriptors = new List<ResourceItemDescriptor>();
            _expireDt = DateTime.Now.AddYears( 1 );
        }

        public ResourceDescriptor (DateTime expireDt)
        {
            _resourceItemDescriptors = new List<ResourceItemDescriptor>();
            _expireDt = expireDt;
        }

        public DateTime GetExpireDt ()
        {
            return _expireDt;
        }

        public ResourceDescriptor Add( ResourceItemDescriptor resourceItemDescriptor )
        {
            _resourceItemDescriptors.Add( resourceItemDescriptor );

            return this;
        }

        public ResourceDescriptor Remove ( string name )
        {
            var item = _resourceItemDescriptors.FirstOrDefault( t => t.Name == name );

            if (item != null)
            {
                _resourceItemDescriptors.Remove( item );
            }

            return this;
        }

        public bool IsValid (string resourceName, string action )
        {
            return _resourceItemDescriptors.Any( t => t.IsValid( resourceName, action ) );
        }

        public override string ToString ()
        {
            return _expireDt.ToString() + "\f" + string.Join( "\v", _resourceItemDescriptors.Select(t=>t.ToString()) );
        }

        public static ResourceDescriptor FromString ( string str )
        {
            var arr = str.Split( '\f' );
            if (arr.Length != 2) return null;


            ResourceDescriptor resourceDescriptor = new ResourceDescriptor();
            resourceDescriptor._expireDt = DateTime.Parse( arr[0]);
            resourceDescriptor._resourceItemDescriptors = arr[1].Split( '\v' ).Select(t=> ResourceItemDescriptor.FromString(t)).Where(t=>t!=null).ToList() ;

            return resourceDescriptor;
        }

        public string ToDtoString ()
        {
            return this.ToString();
        }

        public void FromDtoString ( String dtoString )
        {
            var data = FromString( dtoString );
            if (data != null)
            {
                this._expireDt = data._expireDt;
                this._resourceItemDescriptors = data._resourceItemDescriptors;
            } 
        }
    }

    /// <summary>
    /// 单个资源描述
    /// </summary>
    public class ResourceItemDescriptor
    {
        /// <summary>
        /// 资源名称
        /// e.g. service:fileservice,page:order
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 允许操作
        /// e.g. create,modify,delete
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 限定条件,以冒号分割,格式不正确的将被跳过
        /// e.g. inhours:09-18,expire:2018-12-12 00:00:00
        /// </summary>
        public string[] Filters { get; set; }

        public ResourceItemDescriptor () { }

        public ResourceItemDescriptor(string name,string action )
        {
            this.Name = name;
            this.Action = action;
            this.Filters = new string[] { };
        }

        public ResourceItemDescriptor ( string name, string action,string[] filters )
        {
            this.Name = name;
            this.Action = action;
            this.Filters = filters??new string[] { };
        }

        public bool IsValid ( string resourceName, string action )
        {
            return Name == resourceName && (Action ?? "").Split( ',' ).Contains( action );
        }

        public override string ToString ()
        {
            return Name+"\n"+Action+"\n"+string.Join("\r",Filters);
        }

        public static ResourceItemDescriptor FromString(string str )
        {
            var arr = str.Split( '\n' );
            if (arr.Length != 3) return null;


            ResourceItemDescriptor resourceItemDescriptor = new ResourceItemDescriptor();
            resourceItemDescriptor.Name = arr[0];
            resourceItemDescriptor.Action = arr[1];
            resourceItemDescriptor.Filters = arr[2].Split( '\r' );

            return resourceItemDescriptor;
        }
    }
}
