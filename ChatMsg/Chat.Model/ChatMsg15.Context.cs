﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chat.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class CrmChat14Entities : DbContext
    {
        public CrmChat14Entities()
            : base("name=CrmChat14Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ChatMsg> ChatMsg { get; set; }
        public DbSet<RouteMsgTable> RouteMsgTable { get; set; }
    
        public virtual ObjectResult<Nullable<int>> Usp_GetTotalCount15(string tablename)
        {
            var tablenameParameter = tablename != null ?
                new ObjectParameter("tablename", tablename) :
                new ObjectParameter("tablename", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("Usp_GetTotalCount15", tablenameParameter);
        }
    
        public virtual int Usp_CreateTable15(string tableName, Nullable<System.DateTime> begintime)
        {
            var tableNameParameter = tableName != null ?
                new ObjectParameter("TableName", tableName) :
                new ObjectParameter("TableName", typeof(string));
    
            var begintimeParameter = begintime.HasValue ?
                new ObjectParameter("begintime", begintime) :
                new ObjectParameter("begintime", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Usp_CreateTable15", tableNameParameter, begintimeParameter);
        }
    
        public virtual ObjectResult<Usp_GetHistroryMsg15_Result> Usp_GetHistroryMsg15(Nullable<int> userid, Nullable<System.DateTime> begintime, Nullable<System.DateTime> endtime)
        {
            var useridParameter = userid.HasValue ?
                new ObjectParameter("userid", userid) :
                new ObjectParameter("userid", typeof(int));
    
            var begintimeParameter = begintime.HasValue ?
                new ObjectParameter("begintime", begintime) :
                new ObjectParameter("begintime", typeof(System.DateTime));
    
            var endtimeParameter = endtime.HasValue ?
                new ObjectParameter("endtime", endtime) :
                new ObjectParameter("endtime", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Usp_GetHistroryMsg15_Result>("Usp_GetHistroryMsg15", useridParameter, begintimeParameter, endtimeParameter);
        }
    }
}