﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int occupied = 0;
        private int available = 100;
        private int numVehicles = 0;
        private int indexSelect;

        private void buttonAddData_Click(object sender, EventArgs e)
        {
            // inc and dec the occupied and available spaces
            occupied += 1;
            available -= 1;

            // add rows from inputs.
            int n = dataGridViewParking.Rows.Add();
            dataGridViewParking.Rows[n].Cells[0].Value = textBoxLP.Text;
            dataGridViewParking.Rows[n].Cells[1].Value = textBoxBrand.Text;
            dataGridViewParking.Rows[n].Cells[2].Value = textBoxColor.Text;
            dataGridViewParking.Rows[n].Cells[3].Value = timeTB_TimeIN.Text;
            dataGridViewParking.Rows[n].Cells[4].Value = "NA";
            dataGridViewParking.Rows[n].Cells[5].Value = "NA";
            dataGridViewParking.Rows[n].Cells[6].Value = "NA";
            dataGridViewParking.Rows[n].Cells[7].Value = "FALSE";

            // can be a function. clears text in textbox.
            textBoxLP.Clear();
            textBoxColor.Clear();
            textBoxBrand.Clear();
            textBoxDuration.Clear();
            textBoxAmount.Clear();

            // mark it yellow as pending
            for (int i = 0; i <= 7; i++)
                dataGridViewParking.Rows[n].Cells[i].Style.BackColor = Color.Yellow;
        }

        private void buttonTimeOUT_Click(object sender, EventArgs e)
        {
            DateTime timein = DateTime.Parse(timeTB_TimeIN.Text);
            DateTime timeout = DateTime.Parse(timeTB_TimeOUT.Text);

            available += 1;
            occupied -= 1;

            int duration = int.Parse((timeout - timein).TotalMinutes.ToString());
            var span = TimeSpan.FromMinutes(duration);
            var hour = ((int)span.TotalHours).ToString();
            var Minute = span.Minutes.ToString();

            textBoxDuration.Text = $"{hour}hr {Minute}min";
            double numHours = duration / 60;
            int price;

            if (numHours >= 0)
            {
                if (duration / 60 <= 0.25)
                {
                    textBoxAmount.Text = "₱" + 0;
                }
                else if (numHours <= 3 && numHours > 0.25)
                {
                    textBoxAmount.Text = "₱" + 50;
                }
                else if (numHours > 3)
                {
                    price = 50 + ((int)Math.Ceiling(numHours) - 3) * 20;
                    textBoxAmount.Text = "₱" + price;
                }
                else if (duration == 0)
                {
                    textBoxAmount.Text = "₱" + 0;
                }
            }

            numVehicles += 1;

            dataGridViewParking.Rows[dataGridViewParking.CurrentRow.Index].Cells["timeoutCol"].Value = timeTB_TimeOUT.Text;
            dataGridViewParking.Rows[dataGridViewParking.CurrentRow.Index].Cells["durationCol"].Value = textBoxDuration.Text;
            dataGridViewParking.Rows[dataGridViewParking.CurrentRow.Index].Cells["amountCol"].Value = textBoxAmount.Text;

            for (int j = 0; j <= 7; j++)
            {
                dataGridViewParking.Rows[dataGridViewParking.CurrentRow.Index].Cells[j].Style.BackColor = Color.Red;
            }

            textBoxDuration.Clear();
            textBoxAmount.Clear();
        }

        private void buttonUpdateData_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridViewParking.Rows[indexSelect];
            row.Cells[0].Value = textBoxLP.Text;
            row.Cells[1].Value = textBoxBrand.Text;
            row.Cells[2].Value = textBoxColor.Text;
        }

        private void dataGridViewParking_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // cell click event
            // click only rows not columns

            indexSelect = e.RowIndex;
            DataGridViewRow row = dataGridViewParking.Rows[indexSelect];

            var LP = row.Cells[0].Value ?? " ";
            var Brand = row.Cells[1].Value ?? " ";
            var Color = row.Cells[2].Value ?? " ";

            textBoxLP.Text = LP.ToString();
            textBoxBrand.Text = Brand.ToString();
            textBoxColor.Text = Color.ToString();
        }

        private void buttonDeleteData_Click(object sender, EventArgs e)
        {
            DateTime intime = DateTime.Parse(timeTB_TimeIN.Text);
            DateTime outtime = DateTime.Parse(timeTB_TimeOUT.Text);

            if (dataGridViewParking.Rows.Count > 0)
            {
                double time = double.Parse((outtime - intime).TotalMinutes.ToString());

                if (time <= 15) // limit only to below 15 time
                {
                    available += 1;
                    occupied -= 1;

                    dataGridViewParking.Rows.RemoveAt(this.dataGridViewParking.CurrentRow.Index);
                }
            }
        }

        private void buttonVehiclePaid_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridViewParking.Rows[indexSelect];
            row.Cells[7].Value = "TRUE";
        }

        private void buttonSaveDataDB_Click(object sender, EventArgs e)
        {
            string cs = @"Data Source=.\sqlexpress;Initial Catalog=parkingdb;Integrated Security=True";
            SqlConnection con = new SqlConnection(cs);

            for (int i = 0; i < dataGridViewParking.Rows.Count - 1; i++)
            {
                var licensePlate = dataGridViewParking.Rows[i].Cells[0].Value;
                var brand = dataGridViewParking.Rows[i].Cells[1].Value;
                var color = dataGridViewParking.Rows[i].Cells[2].Value;
                var timein = dataGridViewParking.Rows[i].Cells[3].Value;
                var timeout = dataGridViewParking.Rows[i].Cells[4].Value;
                var duration = dataGridViewParking.Rows[i].Cells[5].Value;
                var amount = dataGridViewParking.Rows[i].Cells[6].Value;
                var paid = dataGridViewParking.Rows[i].Cells[7].Value;

                SqlCommand cmd = new SqlCommand(@"INSERT INTO PARKINGDB
                (licensePlate, brand, color, timeIN, timeOUT, duration, amount, paid)
                VALUES ('"+licensePlate+ "','" + brand + "','" + color + "','" + timein + "','" + timeout + "','" + duration + "', '" + amount + "', '" + paid + "')", con);

                con.Open();
                cmd.ExecuteNonQuery();
                
            }

            MessageBox.Show("All data saved on database!");
        }
    }
}
