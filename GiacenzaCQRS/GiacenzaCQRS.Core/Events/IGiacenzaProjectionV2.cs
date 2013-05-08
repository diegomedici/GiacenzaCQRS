using System;
using System.Data;
using System.Data.SqlClient;

namespace GiacenzaCQRS.Core.Events
{
    public interface IGiacenzaProjectionV2
    {
        void Create(Guid id, string minsan);
        void Carica(string minsan, int quantita);
        void Scarica(string minsan, int quantita);
    }

    public class GiacenzaProjectionV2 : IGiacenzaProjectionV2
    {
        private readonly string _connection;
        private string _createCmdString = "INSERT INTO GiacenzaView (Id, Minsan, Quantita) VALUES ('{0}','{1}',0)";

        public GiacenzaProjectionV2(string connection)
        {
            _connection = connection;
        }

        public void Create(Guid id, string minsan)
        {
            var cmdString = string.Format(_createCmdString, id, minsan);
            var sqlCommand = new SqlCommand(cmdString);
            sqlCommand.CommandType = CommandType.Text;

            using (var conn = new SqlConnection(_connection))
            {
                conn.Open();
                sqlCommand.Connection = conn;
                sqlCommand.ExecuteNonQuery();
            }
        }

        public void Carica(string minsan, int quantita)
        {
            throw new NotImplementedException();
        }

        public void Scarica(string minsan, int quantita)
        {
            throw new NotImplementedException();
        }
    }
}