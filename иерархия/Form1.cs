using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace иерархия
{
    public partial class Form1 : Form
    {
        public Form1()
        {
                InitializeComponent();
                initialization_subd();
        }



        public void initialization_subd()
        {
            var connStr = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5432,
                Username = "postgres",
                Password = "postgres",
                Database = "laba_1"
            }.ConnectionString;

            using (var conn = new NpgsqlConnection(connStr))
            {

                conn.Open();

                var command = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = @"SELECT *
                                        FROM  country_branch as c
                                        LEFT JOIN city_branch as s ON c.id = s.id_country
                                        LEFT JOIN branches as i ON i.id_city = s.id
                                    ORDER BY c.country, s.city, i.branch"

                };

                using (var reader = command.ExecuteReader())
                {

                    var lastAddedNode = new TreeNode();

                    while (reader.Read())
                    {
                            string countryName, cityName, street_name;
                            int count_worker;
                            try
                            {
                                countryName = (string)reader["country"];
                            }
                            catch
                            {
                                countryName = "";
                            }
                            try
                            {
                                cityName = (string)reader["city"];
                            }
                            catch
                            {
                                cityName = "";
                            }
                            try
                            {
                                street_name = (string)reader["branch"];
                                count_worker = (int)reader["count_worker"];
                            }
                            catch
                            {
                                street_name = "";
                                count_worker = 0;
                            }

                            if (lastAddedNode.Text != countryName)
                            {
                                lastAddedNode = treeView1.Nodes.Add(countryName);
                                treeView1.Nodes[treeView1.Nodes.Count - 1].Name = countryName;
                            }

                            if (lastAddedNode.Nodes.Find(cityName, false).Length == 0 && cityName!="")
                            {
                                lastAddedNode.Nodes.Add(cityName);
                                lastAddedNode.Nodes[lastAddedNode.Nodes.Count - 1].Name = cityName;
                            }
                            if(street_name!="")
                                lastAddedNode.Nodes.Find(cityName, false)[0].Nodes.Add(street_name + ". Число работников: " + count_worker);

                    }
                }

                conn.Close();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
