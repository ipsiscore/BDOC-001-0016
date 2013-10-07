using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SIO.Services.Data.Services
{
    public class SQLDataManager : IDisposable
    {
        #region Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        private List<SqlParameter> parameters = new List<SqlParameter>();
        private SqlConnection connection = new SqlConnection();
        private SqlCommand command = new SqlCommand();
        private SqlConnectionStringBuilder myConnBuilder = new SqlConnectionStringBuilder();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Lista de parametros
        /// </summary>
        public List<SqlParameter> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Crea una nueva instancia de la clase <c>SQLDataManager</c>.</para>
        /// </summary>
        public SQLDataManager()
        {
        }

        #endregion Constructors

        #region Methods

        #region Publics

        /// <summary>
        /// <para>Ejecuta un procedimiento almacenado de Transact-SQL en la conexión y devuelve el número de filas afectadas.</para>
        /// </summary>
        /// <param name="storeProcedure">Nombre del procedimiento almacenado que se ejecutará.</param>
        /// <remarks>
        /// <para>Es necesario asignar la cedena de conexión y los parametros correspondientes para poder invocar este método.</para>
        /// <para>Se debe hacer uso de la propiedad <c>Parameters</c> para asignar los parametros del procedimiento almacenado.</para>
        /// </remarks>
        /// <returns>Devuelve un <c>int32</c> que indica el número de filas afectadas.</returns>
        /// <exception cref="SqlException"> Si ocurrió un error en la conexión.</exception>
        public int ExecuteNonQueryProcedure(string storeProcedure)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.ConnectionString = GetStringConnection();
                    connection.Open();
                }

                if (storeProcedure == string.Empty)
                    throw new Exception("El storeProcedure esta vacio");

                command = new SqlCommand(storeProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Clear();
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                return command.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// <para>Ejecuta una instrucción de consulta en la conexión y devuelve el resultado en un <c>DataTable</c>.</para>
        /// </summary>
        /// <param name="query">Instrucción de consulta que será ejecutada.</param>
        /// <remarks><para>Es necesario asignar la cedena de conexión para poder invocar este método.</para></remarks>
        /// <returns>Devuelve un <c>DataTable</c> con los resultados de la consulta.</returns>
        /// <exception cref="SqlException"> Si ocurrió un error en la conexión.</exception>
        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.ConnectionString = GetStringConnection();
                    connection.Open();
                }

                if (query == string.Empty)
                    throw new Exception("El query esta vacio");

                command.CommandType = CommandType.Text;
                command.CommandText = query;
                command.Connection = connection;
                SqlDataReader reader = command.ExecuteReader();

                dt.Load(reader);
                return dt;
            }
            catch
            {
                throw;
            }
            finally
            {
                dt.Dispose();
            }
        }

        /// <summary>
        /// <para>Ejecuta un procedimiento almacenado de consulta en la conexión y devuelve el resultado en un <c>DataTable</c>.</para>
        /// </summary>
        /// <param name="storeProcedure">Nombre del procedimiento almacenado que se ejecutará.</param>
        /// <remarks>
        /// <para>Es necesario asignar la cedena de conexión y los parametros correspondientes para poder invocar este método.</para>
        /// <para>Se debe hacer uso de la propiedad <c>Parameters</c> para asignar los parametros del procedimiento almacenado.</para>
        /// </remarks>
        /// <returns>Devuelve un <c>DataTable</c> con los resultados de la consulta.</returns>
        /// <exception cref="SqlException"> Si ocurrió un error en la conexión.</exception>
        public DataTable ExecuteProcedure(string storeProcedure)
        {
            SqlDataAdapter adapter = null;
            DataSet ds = new DataSet();

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.ConnectionString = GetStringConnection();
                    connection.Open();
                }

                if (storeProcedure == string.Empty)
                    throw new Exception("El storeProcedure esta vacio");

                command = new SqlCommand(storeProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Clear();
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
                if (ds.Tables.Count != 0)
                    return ds.Tables[0];
                else
                    return new DataTable();
            }
            catch
            {
                throw;
            }
            finally
            {
                ds.Dispose();
                adapter.Dispose();
            }
        }

        /// <summary>
        /// <para>Ejecuta un procedimiento almacenado de consulta en la conexión y devuelve el resultado en un <c>DataTable</c>.</para>
        /// </summary>
        /// <param name="storeProcedure">Nombre del procedimiento almacenado que se ejecutará.</param>
        /// <param name="parameters">Lista externa de <c>SqlParameter</c></param>
        /// <remarks>
        /// <para>Es necesario asignar los parametros correspondientes para poder invocar este método.</para>
        /// <para>Se debe hacer uso de la propiedad <c>Parameters</c> para asignar los parametros del procedimiento almacenado.</para>
        /// </remarks>
        /// <returns>Devuelve un <c>DataTable</c> con los resultados de la consulta.</returns>
        /// <exception cref="SqlException"> Si ocurrió un error en la conexión.</exception>
        public DataTable ExecuteProcedure(string storeProcedure, List<SqlParameter> parameters)
        {
            SqlDataAdapter adapter = null;
            DataSet ds = new DataSet();

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.ConnectionString = GetStringConnection();
                    connection.Open();
                }

                if (storeProcedure == string.Empty)
                    throw new Exception("El storeProcedure esta vacio");

                command = new SqlCommand(storeProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Clear();
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
                if (ds.Tables.Count != 0)
                    return ds.Tables[0];
                else
                    return new DataTable();
            }
            catch
            {
                throw;
            }
            finally
            {
                ds.Dispose();
                adapter.Dispose();
            }
        }

        /// <summary>
        /// Ejecuta un Procedimiento Almacenado con los nombre de parámetros específicados y sus valores.
        /// </summary>
        /// <param name="storeProcedure">Nombre del Procedimiento Almacenado.</param>
        /// <param name="parameterNames">Arreglo con los nombres de los parámetros.</param>
        /// <param name="parameterValues">Arreglo con los objectos que contienen los valores de los parámetros.</param>
        /// <returns>Tabla con los resultados.</returns>
        public DataTable ExecuteProcedure(string storeProcedure, string[] parameterNames, object[] parameterValues)
        {
            SqlDataAdapter adapter = null;
            DataTable dt = new DataTable();

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.ConnectionString = GetStringConnection();
                    connection.Open();
                }

                if (storeProcedure == string.Empty)
                    throw new Exception("El storeProcedure esta vacio");
                command = new SqlCommand(storeProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < parameterValues.Length; i++)
                    command.Parameters.Add(new SqlParameter(parameterNames[i], parameterValues[i]));
                adapter = new SqlDataAdapter(command);

                adapter.Fill(dt);
                return dt;
            }
            catch
            {
                throw;
            }
            finally
            {
                dt.Dispose();
                adapter.Dispose();
            }
        }

        /// <summary>
        /// Ejecuta la consulta y devuelve la primera columna de la primera fila del conjunto de resultados que devuelve la consulta.
        /// </summary>
        /// <param name="storeProcedure">Nombre del procedimiento almacenado que se ejecutará.</param>
        /// <returns>Devuelve un valor de tipo <see cref="System.Int32"/> que indica el valor de la primera columna de la primera fila.</returns>
        public int ExecuteScalarProcedure(string storeProcedure)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.ConnectionString = GetStringConnection();
                    connection.Open();
                }

                if (storeProcedure == string.Empty)
                    throw new Exception("El storeProcedure esta vacio");

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = storeProcedure.Replace("'", "");
                command.Parameters.Clear();
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                command.Connection = connection;
                return (int)command.ExecuteScalar();
            }
            catch
            {
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                command.Dispose();
                connection.Close();
            }

            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Publics

        #region Privates

        /// <summary>
        /// Obtiene la cadena de conexión personalizada.
        /// </summary>
        /// <returns></returns>
        private string GetStringConnection()
        {
            try
            {
                myConnBuilder.ConnectionString = connectionString;
                myConnBuilder.ConnectTimeout = 30;
                myConnBuilder.MinPoolSize = 5;
                myConnBuilder.MaxPoolSize = 60;
                myConnBuilder.Pooling = true;
                return myConnBuilder.ConnectionString;
            }
            catch (Exception)
            {
                return connectionString;
            }
        }

        /// <summary>
        /// Agrega el prefijo @ a los elementeos de la colección especificada.
        /// </summary>
        /// <param name="parameterNames">Collección de strings que contienen los nombres de parametros.</param>
        /// <returns>Devuelve la colección espeficada agragan el el prefijo @.</returns>
        private string[] AddATSymbolToStringArray(string[] parameterNames)
        {
            string[] parameterNamesWithAt = new string[parameterNames.Length];
            for (int i = 0; i < parameterNames.Length; i++)
                parameterNamesWithAt[i] = string.Format("@{0}", parameterNames[i]);
            return parameterNamesWithAt;
        }

        /// <summary>
        /// Devuelve un arreglo con el formato <c>string = @string</c> tomando los elementos de la colección proporcionada como base.
        /// </summary>
        /// <param name="parameterNames">Collección de strings que contienen los nombres de parametros.</param>
        /// <remarks>
        /// <para>Para poder utilizar el método debe proporcionarse una collección de elementos válidos como nombre de parámetros.</para>
        /// </remarks>
        /// <returns>Devuelve la colleción de elementos con el formato aplicado.</returns>
        private string[] BuildNameValuePairs(string[] parameterNames)
        {
            string[] parameterPairs = new string[parameterNames.Length];
            for (int i = 0; i < parameterNames.Length; i++)
                parameterPairs[i] = string.Format("{0} = @{0}", parameterNames[i]);
            return parameterPairs;
        }

        #endregion Privates

        #endregion Methods
    }
}