using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GiacenzaCQRS.Core.Events
{
    public interface IGiacenzaProjectionV2
    {
        void Create(Guid id, string minsan, int versione, string originalStreamId);
        void Carica(string minsan, int quantita, int versione);
        void Scarica(string minsan, int quantita, int versione);
        void Aggiorna(string minsan, int quantita, int versione, string originalStreamId);
    }

    public class GiacenzaProjectionV2 : IGiacenzaProjectionV2
    {
        private readonly string _connection;

        private string _createStreamPositions = "insert into StreamPositions (StreamId, Version) values ('{0}', 0)";
        private string _selectStreamPositions = "select Version from StreamPositions where StreamId = '{0}'";
        private string _updateStreamPositions = "update StreamPositions set Version = {0} where StreamId ='{1}'";

        private string _createCmdString = "INSERT INTO GiacenzaView (Id, Minsan, Quantita) VALUES ('{0}','{1}',0)";
        private string _updateCmdString = "Update GiacenzaView set Quantita={0} where minsan='{1}'";
        
        private Dictionary<string, int> _streamLevels = new Dictionary<string, int>();

        public GiacenzaProjectionV2(string connection)
        {
            _connection = connection;
        }

        public void Create(Guid id, string minsan, int versione, string originalStreamId)
        {
            if (GetStreamVersion(originalStreamId) < versione)
            {
                var cmdString = string.Format(_createCmdString, id, minsan, versione);
                var sqlCommand = new SqlCommand(cmdString);
                sqlCommand.CommandType = CommandType.Text;

                using (var conn = new SqlConnection(_connection))
                {
                    conn.Open();
                    using (var trnx = conn.BeginTransaction())
                    {
                        sqlCommand.Transaction = trnx;
                        sqlCommand.Connection = conn;
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = string.Format(_updateStreamPositions, versione, originalStreamId);
                        sqlCommand.ExecuteNonQuery();
                        trnx.Commit();
                    }
                }
                _streamLevels[originalStreamId] = versione;
            }
        }

        private int GetStreamVersion(string streamId)
        {
            if (_streamLevels.ContainsKey(streamId)) return _streamLevels[streamId];
            _streamLevels[streamId] = GetLastSavedVersion(streamId);
            return _streamLevels[streamId];
        }


        private int GetLastSavedVersion(string streamId)
        {
          
            var cmdString = string.Format(_selectStreamPositions, streamId);
            var sqlCommand = new SqlCommand(cmdString);
            sqlCommand.CommandType = CommandType.Text;

            using (var conn = new SqlConnection(_connection))
            {
                conn.Open();
                sqlCommand.Connection = conn;
                object o = sqlCommand.ExecuteScalar();
                if (o == null)
                {
                    sqlCommand.CommandText = string.Format(_createStreamPositions, streamId);
                    sqlCommand.ExecuteNonQuery();
                    return 0;
                }
                return Convert.ToInt32(o);
            }
        }

        public void Carica(string minsan, int quantita, int versione)
        {
            throw new NotImplementedException();
        }

        public void Scarica(string minsan, int quantita, int versione)
        {
            throw new NotImplementedException();
        }

        public void Aggiorna(string minsan, int quantita, int versione, string originalStreamId)
        {
            if (GetStreamVersion(originalStreamId) < versione)
            {
                var cmdString = string.Format(_updateCmdString, quantita, minsan);
                var sqlCommand = new SqlCommand(cmdString);
                sqlCommand.CommandType = CommandType.Text;

                using (var conn = new SqlConnection(_connection))
                {
                    conn.Open();
                    using (var trnx = conn.BeginTransaction())
                    {
                        sqlCommand.Transaction = trnx;
                        sqlCommand.Connection = conn;
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = string.Format(_updateStreamPositions, versione, originalStreamId);
                        sqlCommand.ExecuteNonQuery();
                        trnx.Commit();
                    }
                }
                _streamLevels[originalStreamId] = versione;

            }
        }
    }
}