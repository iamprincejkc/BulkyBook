using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class SP_Call : ISP_Call
    {
        private readonly ApplicationDbContext _dbContext;
        private static string ConnectionString = "";
        public SP_Call(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            ConnectionString = dbContext.Database.GetDbConnection().ConnectionString;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void Execute(string procedureName, DynamicParameters? parameters = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            sqlConnection.Execute(procedureName, parameters,commandType:CommandType.StoredProcedure);
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters? parameters = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            return sqlConnection.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters? parameters = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            var result = SqlMapper.QueryMultiple(sqlConnection, procedureName, parameters, commandType: CommandType.StoredProcedure);
            var item1 = result.Read<T1>().ToList();
            var item2 = result.Read<T2>().ToList();
            if(item1 != null && item2 != null)
            {
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
        }

        public T OneRecord<T>(string procedureName, DynamicParameters? parameters = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            var value = sqlConnection.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public T Single<T>(string procedureName, DynamicParameters? parameters = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            object value = sqlConnection.ExecuteScalar(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"Cannot convert value '{value}' to type '{typeof(T).Name}'.");
            }
            catch (FormatException)
            {
                throw new FormatException($"Cannot convert value '{value}' to type '{typeof(T).Name}' due to invalid format.");
            }
            catch (OverflowException)
            {
                throw new OverflowException($"Cannot convert value '{value}' to type '{typeof(T).Name}' due to overflow.");
            }
        }
    }
}
