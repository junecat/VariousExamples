using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SqlForLearn
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Shown += Form1_Shown;
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection cn = new SqlConnection(sqlConnectionString))
            {
                cn.Open();
                const string sqlCalc = "SELECT SUM(time_of) FROM Plan_db WHERE fio=@fio AND semestr=@semestr";
                using (SqlCommand cmd = new SqlCommand(sqlCalc, cn))
                {
                    cmd.Parameters.Add("@fio", DbType.String).Value = comboBox1.SelectedItem;
                    cmd.Parameters.Add("@semestr", DbType.Int32).Value = Convert.ToInt32(comboBox2.SelectedItem);
                    var rez = cmd.ExecuteScalar();
                    textBox1.Text = rez.ToString();
                }



            }
        }

            const string sqlConnectionString = "Password=rem;Persist Security Info=False;User ID=davydov;Initial Catalog=ExperimentsDB;Data Source=172.19.110.215";

        private void Form1_Shown(object sender, EventArgs e)
        {
            // заполняем выпадающие списки - преподавателей и семестров
            using ( SqlConnection cn = new SqlConnection(sqlConnectionString))
            {
                cn.Open();
                const string sqlSelPeoples = "SELECT DISTINCT fio FROM Plan_db ORDER BY fio";
                using (SqlCommand cmd = new SqlCommand(sqlSelPeoples, cn))
                {
                    comboBox1.Items.Clear();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            comboBox1.Items.Add(reader.GetString(0));
                        }
                    }
                    if (comboBox1.Items.Count > 0)
                        comboBox1.SelectedIndex = 0;
                }

            }

            using (SqlConnection cn = new SqlConnection(sqlConnectionString))
            {
                cn.Open();
                const string sqlSelSemestr = "SELECT DISTINCT semestr FROM Plan_db ORDER BY semestr";
                using (SqlCommand cmd = new SqlCommand(sqlSelSemestr, cn))
                {
                    comboBox2.Items.Clear();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox2.Items.Add(reader.GetInt32(0));
                        }
                    }
                    if (comboBox2.Items.Count > 0)
                        comboBox2.SelectedIndex = 0;
                }

            }



        }


    }
}
