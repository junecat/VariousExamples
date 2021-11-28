using MySqlConnector;
using System;
using System.Windows.Forms;
using System.IO;

namespace MysqlBlobExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
        }

        // CREATE TABLE `testdb`.`pictures` (
        // `id` INT NOT NULL AUTO_INCREMENT,
        // `pixblob` LONGBLOB NOT NULL,
        // PRIMARY KEY(`id`));

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = "192.168.0.101",
                    UserID = "konst",
                    Password = "<password>",
                    Database = "testdb",
                };
                string cnString = builder.ConnectionString;
                // open a connection
                using (var connection = new MySqlConnection(cnString))
                {
                    connection.Open();

                    byte[] imageBytes = File.ReadAllBytes("nature.jpg");

                    // create a DB command and set the SQL statement with parameters
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"INSERT INTO pictures (pixblob) VALUES (@blob);";
                        command.Parameters.Add("@blob", MySqlDbType.LongBlob).Value = imageBytes;
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Done!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
