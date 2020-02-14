using System;
using System.Data;
using System.Threading.Tasks;
using CacheStack;
using Canducci.SqlKata.Dapper.Extensions.Builder;
using Canducci.SqlKata.Dapper.SqlServer;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace RedisCacheLib.Infrastructure
{
	public interface IDataRepositoryBase<T> where T : class
	{
		Task<T> SaveAsync(T item);
		Task<T> GetByIdOrDefaultAsync(int id);
		Task SoftDeleteAsync(T item);
		Task SoftUndeleteAsync(T item);
	}

	public abstract class DataRepositoryBase<T> : IDataRepositoryBase<T> where T : class
	{
		protected IDbConnection Db { get; set; }

		protected DataRepositoryBase(IDbConnection db)
		{
			Db = db;
		}

		protected object GetId(T item)
		{
			var type = item.GetType();
			var idProperty = type.GetProperty("Id");
			if (idProperty == null)
				throw new Exception("Object does not have Id property. Type: " + type);

			return idProperty.GetValue(item);
		}

		public virtual async Task<T> SaveAsync(T item)
		{
			//var savedItem = Db.SoftBuild();

			var id = GetId(item);

			//return savedItem;
			return item;
		}

		public virtual async Task<T> GetByIdOrDefaultAsync(int id)
		{
			//return await Db.SoftBuild().QuerySingleOrDefaultAsync<T>();
			return null;
		}

		public virtual async Task SoftDeleteAsync(T item)
		{
			var id = GetId(item);

			//Db.Execute("update [{0}] set IsDeleted=1 where Id=@Id".Fmt(
			//	Db.GetTableName<T>()
			//), new
			//{
			//	id
			//});
		}

		public virtual async Task SoftUndeleteAsync(T item)
		{
			var id = GetId(item);

			//Db.Execute("update [{0}] set IsDeleted=0 where Id=@Id".Fmt(
			//	Db.GetTableName<T>()
			//), new
			//{
			//	id
			//});
		}
	}
}