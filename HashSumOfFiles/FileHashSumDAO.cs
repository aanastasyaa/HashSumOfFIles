using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace HashSumOfFiles
{
    /*
        Предоставляет метод для сохранения хэш-сумм файлов, получаемых из очереди, в БД        
    */
    public class FileHashSumDAO
    {
        private MyConcurrentQueue<FileHashSum> fileHashSums;

        public FileHashSumDAO(MyConcurrentQueue<FileHashSum> fileHashSums)
        {
            this.fileHashSums = fileHashSums;
            // ClearTable();
        }
                
        private void ClearTable()
        {
            string sql = "DELETE FROM hashsums";
            using (OracleCommand cmd = new OracleCommand(sql))
            {
                OracleConnection conn = DBUtils.GetConnection();
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void WriteToDB()
        {
            string sql = "INSERT INTO hashsums (name, error, hashsum) VALUES (:name, :error, :hash)";
            using (OracleCommand cmd = new OracleCommand(sql))
            {
                while (!fileHashSums.IsCompleted()) // пока потоки не закончат добавлять элементы и очередь не пуста
                {
                    FileHashSum file = fileHashSums.Dequeue();
                    if (file == null)
                        break;
                    OracleConnection conn = DBUtils.GetConnection();
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.BindByName = true;
                    cmd.Parameters.Add(new OracleParameter("name", OracleDbType.NVarchar2));
                    cmd.Parameters.Add(new OracleParameter("error", OracleDbType.NVarchar2));
                    cmd.Parameters.Add(new OracleParameter("hash", OracleDbType.NVarchar2));

                    cmd.Parameters[0].Value = file.Filename;
                    cmd.Parameters[1].Value = file.Error;
                    cmd.Parameters[2].Value = file.HashSum;

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                Console.WriteLine("All hashsums in DB");
            }
        }
    }
}
