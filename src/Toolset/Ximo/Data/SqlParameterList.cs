using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace Ximo.Data
{
    public class SqlParameterList : Collection<SqlParameter>
    {
        /// <summary>
        ///     Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        public void AddParameter(string parameterName, object parameterValue, SqlDbType parameterType,
            ParameterDirection parameterDirection)
        {
            var param = new SqlParameter
            {
                ParameterName = parameterName,
                Value = parameterValue ?? DBNull.Value,
                SqlDbType = parameterType,
                Direction = parameterDirection
            };
            Add(param);
        }

        /// <summary>
        ///     Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="paramSize">Size of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        public void AddParameter(string parameterName, object parameterValue, SqlDbType parameterType, int paramSize,
            ParameterDirection parameterDirection)
        {
            var param = new SqlParameter
            {
                ParameterName = parameterName,
                Value = parameterValue ?? DBNull.Value,
                SqlDbType = parameterType,
                Direction = parameterDirection,
                Size = paramSize
            };
            Add(param);
        }

        /// <summary>
        ///     Adds the output parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The optional value of the output parameter.</param>
        public void AddOutputParameter(string parameterName, SqlDbType parameterType, object value = null)
        {
            var param = new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = parameterType,
                Direction = ParameterDirection.Output
            };
            if (value != null)
            {
                param.Value = value;
            }
            Add(param);
        }

        /// <summary>
        ///     Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="paramSize">Size of the parameter.</param>
        public void AddParameter(string parameterName, object parameterValue, SqlDbType parameterType, int paramSize)
        {
            var param = new SqlParameter
            {
                ParameterName = parameterName,
                Value = parameterValue ?? DBNull.Value,
                SqlDbType = parameterType,
                Direction = ParameterDirection.Input,
                Size = paramSize
            };
            Add(param);
        }

        /// <summary>
        ///     Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        public void AddParameter(string parameterName, object parameterValue, SqlDbType parameterType)
        {
            var param = new SqlParameter
            {
                ParameterName = parameterName,
                Value = parameterValue ?? DBNull.Value,
                SqlDbType = parameterType,
                Direction = ParameterDirection.Input
            };
            Add(param);
        }
    }
}