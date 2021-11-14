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
    public partial class FoodInfoForm : Form
    {
        public FoodInfoForm()
        {
            InitializeComponent();
        }
        private void InitValues()
        {
            string connStr = "server=.; database = RestaurantManagement; Integrated Security = true; ";
            SqlConnection conn = new SqlConnection(connStr);

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select ID, Name from Category";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            conn.Open();
            da.Fill(ds, "Category");

            cbCatName.DataSource = ds.Tables["Category"];
            cbCatName.DisplayMember = "Name";
            cbCatName.ValueMember = "ID";

            conn.Close();
            conn.Dispose();
        }
        private void ResetText()
        {
            txtFoodID.ResetText();
            txtFoodName.ResetText();
            txtNotes.ResetText();
            txtFoodUnit.ResetText();
            cbCatName.ResetText();
            nudPrice.ResetText();
        }
        private void FoodInfoForm_Load(object sender, EventArgs e)
        {
            InitValues();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = "server=.; database = RestaurantManagement; Integrated Security = true; ";
                SqlConnection conn = new SqlConnection(connStr);

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "execute [InsertFood] @id output, @name, @unit, @foodCatID, @price, @notes";

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 1000);
                cmd.Parameters.Add("@unit", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@foodCatID", SqlDbType.Int);
                cmd.Parameters.Add("@price", SqlDbType.Int);
                cmd.Parameters.Add("@notes", SqlDbType.NVarChar, 3000);

                cmd.Parameters["@id"].Direction = ParameterDirection.Output;

                cmd.Parameters["@name"].Value = txtFoodName.Text;
                cmd.Parameters["@unit"].Value = txtFoodUnit.Text;
                cmd.Parameters["@foodCatID"].Value = cbCatName.SelectedValue;
                cmd.Parameters["@price"].Value = nudPrice.Value;
                cmd.Parameters["@notes"].Value = txtNotes.Text;

                conn.Open();

                int rowsAff = cmd.ExecuteNonQuery();

                if (rowsAff > 0)
                {
                    string foodID = cmd.Parameters["@id"].Value.ToString();

                    MessageBox.Show("Thêm món ăn thành công! Food ID = " + foodID, "Thông báo");

                    ResetText();
                }
                else
                {
                    MessageBox.Show("Thêm món ăn thất bại!");
                }

                conn.Close();
                conn.Dispose();
            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message, "Lỗi SQL");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Lỗi");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = "server=.; database = RestaurantManagement; Integrated Security = true; ";
                SqlConnection conn = new SqlConnection(connStr);

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "execute [UpdateFood] @id, @name, @unit, @foodCatID, @price, @notes";

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 1000);
                cmd.Parameters.Add("@unit", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@foodCatID", SqlDbType.Int);
                cmd.Parameters.Add("@price", SqlDbType.Int);
                cmd.Parameters.Add("@notes", SqlDbType.NVarChar, 3000);

                cmd.Parameters["@id"].Value = int.Parse(txtFoodID.Text);
                cmd.Parameters["@name"].Value = txtFoodName.Text;
                cmd.Parameters["@unit"].Value = txtFoodUnit.Text;
                cmd.Parameters["@foodCatID"].Value = cbCatName.SelectedValue;
                cmd.Parameters["@price"].Value = nudPrice.Value;
                cmd.Parameters["@notes"].Value = txtNotes.Text;

                conn.Open();

                int rowsAff = cmd.ExecuteNonQuery();

                if (rowsAff > 0)
                {
                    MessageBox.Show("Cập nhật món ăn thành công!", "Thông báo");

                    ResetText();
                }
                else
                {
                    MessageBox.Show("Cập nhật món ăn thất bại!");
                }

                conn.Close();
                conn.Dispose();
            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message, "Lỗi SQL");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Lỗi");
            }
        }

        public void DisplayFoodInfo(DataRowView drv)
        {
            try
            {
                txtFoodID.Text = drv["ID"].ToString();
                txtFoodName.Text = drv["Name"].ToString();
                txtFoodUnit.Text = drv["Unit"].ToString();
                txtNotes.Text = drv["Notes"].ToString();
                nudPrice.Text = drv["Price"].ToString();

                cbCatName.SelectedIndex = -1;

                for (int i = 0; i < cbCatName.Items.Count; i++)
                {
                    DataRowView cat = cbCatName.Items[i] as DataRowView;
                    if (cat["ID"].ToString() == drv["FoodCategoryID"].ToString())
                    {
                        cbCatName.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Lỗi");
                Close();
            }
        }
    }
}
