using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.ConstrainedExecution;

namespace Boeger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnection"].ConnectionString;

        private void Clear()
        {
            txtTitel.Clear();
            txtAuthor.Clear();
            txtYear.Clear();
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            string error = "";
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("INSERT INTO books (title, author, year) VALUES (@title, @author, @year)", connection);
                command.Parameters.Add(CreateParam("@title", txtTitel.Text.Trim(), SqlDbType.NVarChar));
                command.Parameters.Add(CreateParam("@author", txtAuthor.Text.Trim(), SqlDbType.NVarChar));
                command.Parameters.Add(CreateParam("@year", txtYear.Text.Trim(), SqlDbType.Int));
                connection.Open();
                if (command.ExecuteNonQuery() == 1)
                {
                    Clear();
                    return;
                }
                error = "Illegal database operation";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                if (connection != null) connection.Close();
            }
            MessageBox.Show(error);
        }

        private void SearchBook_Click(object sender, RoutedEventArgs e)
        {
            string error = "";
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("SELECT author, year FROM books WHERE title = @title", connection);
                command.Parameters.Add(CreateParam("@title", txtTitel.Text.Trim(), SqlDbType.NVarChar));
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string author = reader["author"].ToString();
                    string year = reader["year"].ToString();
                    MessageBox.Show($"Author: {author}\nYear: {year}", "Book Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Book not found.", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                if (connection != null) connection.Close();
            }

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
            }
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            string error = "";
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("DELETE FROM books WHERE title = @title", connection);
                command.Parameters.Add(CreateParam("@title", txtTitel.Text.Trim(), SqlDbType.NVarChar));
                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Book deleted successfully.", "Delete Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Book not found.", "Delete Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                if (connection != null) connection.Close();
            }

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
            }
        }


        private SqlParameter CreateParam(string name, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter(name, type);
            param.Value = value;
            return param;
        }
    }
}
