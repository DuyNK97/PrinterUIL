using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;

namespace Printer.Class
{
    public class SQLHelper
    {
        private string Constring = ConfigurationManager.ConnectionStrings["connection"].ToString();
        private static int ExecuteNonQuery(string Constring, CommandType type, string SqlString, List<SqlParameter> ParList)
        {
            using (SqlConnection conn = new SqlConnection(Constring))
            {
                using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                {
                    cmd.CommandType = type;
                    if (ParList != null && ParList.Count > 0)
                    {
                        foreach (SqlParameter parameter in ParList)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }

                    try
                    {
                        conn.Open();
                        int val = cmd.ExecuteNonQuery();
                        return val;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        public (DataSet, bool) GetLotinf(string lot)
        {
            DataSet ds = new DataSet();

            try
            {
                //string sql = "SELECT SKU, LOTNO, QTY, SN, TIME,EARN FROM Label_DATA where LOTNO = @lot ";
                string sql = "SELECT SKU, LOTNO, QTY, SN, TIME,EARN FROM DATA where LOTNO = @lot ";

                using (SqlConnection conn = new SqlConnection(Constring))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {

                        cmd.Parameters.AddWithValue("@lot", lot);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);
                    }
                }
                return (ds, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return (ds, false);
            }


        }

        public (DataSet, bool) GetSNinf(string sn)
        {
            DataSet ds = new DataSet();

            try
            {
                //string sql = "SELECT SKU, LOTNO, QTY, SN, TIME,EARN FROM Label_DATA where sn = @sn ";
                string sql = "SELECT SKU, LOTNO, QTY, SN, TIME,EARN FROM DATA where sn = @sn ";

                using (SqlConnection conn = new SqlConnection(Constring))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {

                        cmd.Parameters.AddWithValue("@sn", sn);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);
                    }
                }
                return (ds, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return (ds, false);
            }


        }

        public DataSet SearchData(string sn, string lot, DateTime? datefrom = null, DateTime? dateto = null)
        {
            DataSet ds = new DataSet();

            try
            {
                //string sql = "SELECT EARN, SKU, LOTNO, QTY, SN,UserID, TIME FROM Label_DATA where 1 = 1 ";
                string sql = "SELECT EARN, SKU, LOTNO, QTY, SN,UserID, TIME FROM DATA where 1 = 1 ";
                if (!string.IsNullOrEmpty(sn))
                {
                    sql += "AND  sn like @sn ";
                }
                if (!string.IsNullOrEmpty(lot))
                {
                    sql += "AND  LOTNO like @lot ";
                }
                if (datefrom != null)
                {
                    sql += "AND  time >= @datefrom ";
                }

                if (dateto != null)
                {
                    sql += "AND  time <= @dateto ";
                }
                sql += "ORDER BY time desc";

                using (SqlConnection conn = new SqlConnection(Constring))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {

                        //cmd.Parameters.AddWithValue("@sn", sn);
                        if (sn != null)
                        {
                            //cmd.Parameters.AddWithValue("@sn", sn);
                            cmd.Parameters.AddWithValue("@sn", sn + "%");
                        }
                        if (lot != null)
                        {
                            //cmd.Parameters.AddWithValue("@sn", sn);
                            cmd.Parameters.AddWithValue("@lot", lot + "%");
                        }

                        if (datefrom != null)
                        {
                            cmd.Parameters.AddWithValue("@datefrom", datefrom);
                        }

                        if (dateto != null)
                        {
                            cmd.Parameters.AddWithValue("@dateto", dateto);
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return ds;
            }


        }

        //public bool Savedata( chkdata)
        //{
        //    try
        //    {
        //        //string sql = "INSERT INTO label_DATA (SKU, LOTNO, QTY, SN, [TIME],EARN) values(@sku,@Lotno,@qty,@Serianumber,@time,@earn)";
        //        string sql = "INSERT INTO DATA (SKU, LOTNO, QTY, SN, [TIME],EARN) values(@sku,@Lotno,@qty,@Serianumber,@time,@earn)";
        //        List<SqlParameter> par = new List<SqlParameter>() {
        //             new SqlParameter("@sku",chkdata.SKU),
        //             new SqlParameter("@Lotno",chkdata.LotNo),
        //             new SqlParameter("@qty",chkdata.Qty),
        //             new SqlParameter("@Serianumber",chkdata.Serianumber),
        //             new SqlParameter("@time",chkdata.Time),
        //             new SqlParameter("@earn",chkdata.EARN)
        //             };
        //        int rowsAffected = ExecuteNonQuery(Constring, CommandType.Text, sql, par);
        //        if (rowsAffected > 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.Message);
        //        return false;
        //    }
        //}
        public DataSet GetERN()
        {
            DataSet ds = new DataSet();

            try
            {
                string sql = "select * from EAN ";

                using (SqlConnection conn = new SqlConnection(Constring))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return ds;
            }


        }
        public async Task DeleteDataFromDatabase(List<string> snsToDelete)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Constring))
                {
                    await conn.OpenAsync();
                    foreach (string sn in snsToDelete)
                    {
                        // using (var command = new SqlCommand("DELETE FROM Label_DATA WHERE SN = @SN", conn))
                        using (var command = new SqlCommand("DELETE FROM DATA WHERE SN = @SN", conn))
                        {
                            command.Parameters.AddWithValue("@SN", sn);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task<bool> UpdateData(string sn, string sku, string lotNo, int qty, DateTime time, string earn)
        {
            try
            {
                string sql = "UPDATE DATA SET SKU = @sku, LOTNO = @lotNo, QTY = @qty, TIME = @time, EARN = @earn WHERE SN = @sn";
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@sku", sku),
                    new SqlParameter("@lotNo", lotNo),
                    new SqlParameter("@qty", qty),
                    new SqlParameter("@time", time),
                    new SqlParameter("@earn", earn),
                    new SqlParameter("@sn", sn)
                };

                int rowsAffected = ExecuteNonQuery(Constring, CommandType.Text, sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating data: " + ex.Message);
                return false;
            }
        }



    }
}