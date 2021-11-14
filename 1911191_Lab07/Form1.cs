using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1911191_Lab07
{
    public partial class Form1 : Form
    {
        private DataTable foodDT;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCat();
        }

        private void LoadCat()
        {
            string connStr = "server=.; database = RestaurantManagement; Integrated Security = true; ";
            SqlConnection conn = new SqlConnection(connStr);

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select ID, Name from Category";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            conn.Open();
            da.Fill(dt);

            conn.Close();
            conn.Dispose();

            cbCat.DataSource = dt;
            cbCat.DisplayMember = "Name";
            cbCat.ValueMember = "ID";
        }

        private void cbCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCat.SelectedIndex == -1) return;

            string connStr = "server=.; database = RestaurantManagement; Integrated Security = true; ";
            SqlConnection conn = new SqlConnection(connStr);

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Food where FoodCategoryID = @catID";

            cmd.Parameters.Add("@catID", SqlDbType.Int);

            if (cbCat.SelectedValue is DataRowView)
            {
                DataRowView drv = cbCat.SelectedValue as DataRowView;
                cmd.Parameters["@catID"].Value = drv["ID"];
            }
            else
            {
                cmd.Parameters["@catID"].Value = cbCat.SelectedValue;
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            foodDT = new DataTable();

            conn.Open();
            da.Fill(foodDT);

            conn.Close();
            conn.Dispose();

            dgvFoodList.DataSource = foodDT;
            lbQuant.Text = "Có tất cả " + foodDT.Rows.Count.ToString();
            lbCat.Text = "món ăn thuộc nhóm " + cbCat.Text;
        }

        private void tsmQuant_Click(object sender, EventArgs e)
        {
            string connStr = "server=.; database = RestaurantManagement; Integrated Security = true; ";
            SqlConnection conn = new SqlConnection(connStr);

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select @numSaleFood = sum(Quantity) from BillDetails where FoodID = @foodID";

            if (dgvFoodList.SelectedRows.Count > 0)
            {
                DataGridViewRow selRow = dgvFoodList.SelectedRows[0];

                DataRowView drv = selRow.DataBoundItem as DataRowView;

                cmd.Parameters.Add("@foodID", SqlDbType.Int);
                cmd.Parameters["@foodID"].Value = drv["ID"];

                cmd.Parameters.Add("@numSaleFood", SqlDbType.Int);
                cmd.Parameters["@numSaleFood"].Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();

                string result = cmd.Parameters["@numSaleFood"].Value.Equals(0) ? "0" : cmd.Parameters["@numSaleFood"].Value.ToString();
                MessageBox.Show("Tổng số lượng món " + drv["Name"] + " đã bán là: " + result + " " + drv["Unit"]);

                conn.Close();
            }
            cmd.Dispose();
            conn.Dispose();
        }

        private void tsmAdd_Click(object sender, EventArgs e)
        {
            FoodInfoForm fif = new FoodInfoForm();
            fif.FormClosed += fif_FormClosed;
            fif.Show(this);
        }

        private void fif_FormClosed(object sender, FormClosedEventArgs e)
        {
            int i = cbCat.SelectedIndex;
            cbCat.SelectedIndex = -1;
            cbCat.SelectedIndex = i;
        }

        private void tsmUpdate_Click(object sender, EventArgs e)
        {
            if (dgvFoodList.SelectedRows.Count > 0)
            {
                DataGridViewRow selRow = dgvFoodList.SelectedRows[0];
                DataRowView drv = selRow.DataBoundItem as DataRowView;

                FoodInfoForm fif = new FoodInfoForm();
                fif.FormClosed += fif_FormClosed;

                fif.Show(this);
                fif.DisplayFoodInfo(drv);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (foodDT == null) return;
            
            DataView foodView = new DataView(foodDT, "Name like '%" + txtSearch.Text + "%'", "Price DESC", DataViewRowState.OriginalRows);
            dgvFoodList.DataSource = foodView;
        }
    }
}
