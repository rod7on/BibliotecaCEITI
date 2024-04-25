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
using MaterialSkin;


namespace BibliotecaCEITI
{
    public partial class Form1 : MaterialSkin.Controls.MaterialForm
    {
        string connString = "Data Source=(localdb)\\Local;Initial Catalog=BibliotecaCEITI;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
        }

        private void elevi_Click(object sender, EventArgs e)
        {
            this.panel1.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Obținerea datelor din TextBox-uri
            string denumire = textBox1.Text;
            string numeAutor = textBox2.Text;

            try
            {
                // Deschiderea conexiunii cu baza de date
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    // Crearea instrucțiunii SQL pentru selectarea stock-ului cărții
                    string query = "SELECT Stock FROM Carti WHERE Denumire = @Denumire AND NumeAutor = @NumeAutor";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Adăugarea parametrilor în instrucțiunea SQL pentru a preveni SQL injection
                    command.Parameters.AddWithValue("@Denumire", denumire);
                    command.Parameters.AddWithValue("@NumeAutor", numeAutor);

                    // Executarea instrucțiunii SQL și obținerea rezultatului
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        // Afisarea stock-ului în textBox3
                        textBox3.Text = result.ToString();
                        MessageBox.Show("Stock-ul cărții \"" + denumire + "\" de la autorul \"" + numeAutor + "\" este " + result.ToString(), "Stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        textBox3.Text = ""; // Curățarea textBox3 în cazul în care nu există o înregistrare pentru cartea respectivă
                        MessageBox.Show("Nu există înregistrări pentru cartea \"" + denumire + "\" de la autorul \"" + numeAutor + "\" în baza de date.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la conectarea cu baza de date: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    

        private void AfisareElev_Click(object sender, EventArgs e)
        {
            // Construim interogarea SQL folosind parametri pentru a preveni SQL Injection
            string query = "SELECT * FROM Elevi WHERE Nume = @Nume AND Prenume = @Prenume AND AnulDeStudii = @AnStudii";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Adăugăm parametrii la comandă
                    cmd.Parameters.AddWithValue("@Nume", textBoxNume.Text);
                    cmd.Parameters.AddWithValue("@Prenume", textBoxPrenume.Text);

                    // Determinăm anul de studii selectat
                    int anStudii = 0;
                    if (radioButton1.Checked) anStudii = 1;
                    else if (radioButton2.Checked) anStudii = 2;
                    else if (radioButton3.Checked) anStudii = 3;
                    else if (radioButton4.Checked) anStudii = 4;

                    cmd.Parameters.AddWithValue("@AnStudii", anStudii);

                    // Deschidem conexiunea
                    conn.Open();

                    // Executăm interogarea și obținem un obiect SqlDataReader
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Populăm DataGridView cu datele obținute
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string denumire = textBox1.Text;
            string numeAutor = textBox2.Text;
            int stock;

            // Conversia stock-ului la tipul corect și tratarea erorilor posibile
            if (!int.TryParse(textBox3.Text, out stock))
            {
                MessageBox.Show("Vă rugăm introduceți un număr valid pentru stoc.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            try
            {
                // Deschiderea conexiunii cu baza de date
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    // Crearea instrucțiunii SQL pentru inserarea datelor în baza de date
                    string query = "INSERT INTO Carti (Denumire, NumeAutor, Stock) VALUES (@Denumire, @NumeAutor, @Stock)";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Adăugarea parametrilor în instrucțiunea SQL pentru a preveni SQL injection
                    command.Parameters.AddWithValue("@Denumire", denumire);
                    command.Parameters.AddWithValue("@NumeAutor", numeAutor);
                    command.Parameters.AddWithValue("@Stock", stock);

                    // Executarea instrucțiunii SQL
                    int rowsAffected = command.ExecuteNonQuery();

                    // Verificarea dacă inserarea a avut succes și afișarea unui mesaj corespunzător
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cartea a fost adăugată cu succes în baza de date.", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Adăugarea cărții în baza de date a eșuat.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la conectarea cu baza de date: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Obținerea datelor din TextBox-uri
            string denumire = textBox1.Text;
            string numeAutor = textBox2.Text;
            int stockDeSters;

            // Verificarea dacă cantitatea introdusă în textBox3 este un număr valid
            if (!int.TryParse(textBox3.Text, out stockDeSters))
            {
                MessageBox.Show("Introduceți o valoare validă pentru cantitatea de șters.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Deschiderea conexiunii cu baza de date
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    // Crearea instrucțiunii SQL pentru actualizarea stock-ului cărții
                    string query = "UPDATE Carti SET Stock = Stock - @StockDeSters WHERE Denumire = @Denumire AND NumeAutor = @NumeAutor";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Adăugarea parametrilor în instrucțiunea SQL pentru a preveni SQL injection
                    command.Parameters.AddWithValue("@StockDeSters", stockDeSters);
                    command.Parameters.AddWithValue("@Denumire", denumire);
                    command.Parameters.AddWithValue("@NumeAutor", numeAutor);

                    // Executarea instrucțiunii SQL
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Stock-ul cărții \"" + denumire + "\" de la autorul \"" + numeAutor + "\" a fost actualizat cu succes.", "Actualizare stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nu există înregistrări pentru cartea \"" + denumire + "\" de la autorul \"" + numeAutor + "\" în baza de date sau cantitatea specificată este mai mare decât stock-ul disponibil.", "Actualizare stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A apărut o eroare la conectarea cu baza de date: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
    
