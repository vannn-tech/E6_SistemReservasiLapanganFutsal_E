using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Bagian A: Pengenalan ADO.NET (Namespace yang diperlukan)
using System.Data.SqlClient;

namespace Reservasi_Futsal
{
    public partial class Form1 : Form
    {
        // Connection String - Menggunakan Server LAPTOP-5R80O1Q5\MSSQLSERVER01
        string connectionString = @"Data Source=LAPTOP-5R80O1Q5\MSSQLSERVER01;Initial Catalog=DBFutsalADO;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set txtID menjadi ReadOnly karena ID otomatis dari database
            txtID.ReadOnly = true;

            // Bagian B: Koneksi Database saat aplikasi pertama kali dijalankan
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Jika berhasil terbuka, langsung muat data ke GridView
                }
                catch (Exception ex)
                {
                    // Memberikan informasi jika koneksi gagal saat startup
                    MessageBox.Show("Koneksi Database Gagal saat Startup: " + ex.Message, "Error Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================================
        // FITUR: BUTTON CONNECT (Validasi Koneksi Manual)
        // ==========================================
        private void btnConnect_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Koneksi Database Berhasil!", "Status Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Koneksi Gagal: " + ex.Message, "Error Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Bagian D: Implementasikan ExecuteScalar untuk menghitung total record
        private void HitungTotalRecord()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM Lapangan";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    int total = (int)cmd.ExecuteScalar(); // Bagian D: ExecuteScalar
                    lblTotal.Text = "Total Record: " + total.ToString();
                }
                catch { /* Abaikan jika terjadi error pada perhitungan */ }
            }
        }

        // Bagian E: Tombol Tampilkan Data menggunakan SqlDataReader
        private void btnTampilkan_Click(object sender, EventArgs e)
        {
            TampilkanData();
            MessageBox.Show("Berhasil Menampilkan Data.", "Informasi");
        }

        // Fungsi utama untuk memuat data ke DataGridView
        private void TampilkanData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LapanganID");
            dt.Columns.Add("NamaLapangan");
            dt.Columns.Add("Lokasi");
            dt.Columns.Add("Status");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Lapangan", conn);
                    conn.Open();
                    // Bagian E: Menampilkan data menggunakan SqlDataReader
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        dt.Rows.Add(reader["LapanganID"], reader["NamaLapangan"], reader["Lokasi"], reader["Status"]);
                    }
                    dgvLapangan.DataSource = dt;

                    // Penyesuaian agar baris jadi biru saat diklik & tidak bisa diketik
                    dgvLapangan.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Full Row Biru
                    dgvLapangan.ReadOnly = true; // Tidak bisa diketik/edit di Grid
                    dgvLapangan.AllowUserToAddRows = false; // Menghilangkan baris kosong bawah

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mengambil data: " + ex.Message);
                }
            }
            HitungTotalRecord();
        }

        // Bagian D: Implementasikan query INSERT menggunakan SqlCommand dan ExecuteNonQuery
        private void btnInsert_Click(object sender, EventArgs e)
        {
            // Bagian F: Validasi input (Field penting tidak boleh kosong)
            if (string.IsNullOrEmpty(txtNama.Text) || string.IsNullOrEmpty(cmbStatus.Text))
            {
                MessageBox.Show("Nama dan Status wajib diisi!", "Peringatan");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Lapangan (NamaLapangan, Lokasi, Status) VALUES (@nama, @lokasi, @status)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@lokasi", txtLokasi.Text);
                cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                conn.Open();
                cmd.ExecuteNonQuery(); // Bagian D: ExecuteNonQuery
                MessageBox.Show("Data Lapangan Berhasil Ditambahkan!");
                TampilkanData();
            }
        }

        // Bagian D: Implementasikan query UPDATE menggunakan SqlCommand dan ExecuteNonQuery
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Bagian F: Validasi input (Pastikan data sudah dipilih)
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Pilih data di tabel terlebih dahulu!", "Peringatan");
                return;
            }

            // Bagian F: Tambahkan konfirmasi sebelum data diubah
            DialogResult dr = MessageBox.Show("Yakin ingin mengubah data?", "Konfirmasi Update", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Lapangan SET NamaLapangan=@nama, Lokasi=@lokasi, Status=@status WHERE LapanganID=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", txtID.Text);
                    cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    cmd.Parameters.AddWithValue("@lokasi", txtLokasi.Text);
                    cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Bagian D: ExecuteNonQuery
                    MessageBox.Show("Data Berhasil Diperbarui!");
                    TampilkanData();
                }
            }
        }

        // Bagian D: Implementasikan query DELETE menggunakan SqlCommand dan ExecuteNonQuery
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Pilih data yang akan dihapus!");
                return;
            }

            // Bagian F: Tambahkan konfirmasi sebelum data dihapus
            DialogResult dr = MessageBox.Show("Hapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Lapangan WHERE LapanganID=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", txtID.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Bagian D: ExecuteNonQuery
                    MessageBox.Show("Data Terhapus!");
                    TampilkanData();
                }
            }
        }

        // Bagian E: Fitur pencarian data (Teks, Angka, Kapital/Kecil tetap Valid)
        private void btnCari_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Query menggunakan LIKE untuk pencarian fleksibel
                    // SQL Server secara default tidak membedakan Kapital/Kecil (CI - Case Insensitive)
                    string query = "SELECT * FROM Lapangan WHERE NamaLapangan LIKE @cari OR Lokasi LIKE @cari OR Status LIKE @cari";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Mengambil input dan menghapus spasi di awal/akhir agar pencarian lebih akurat
                    string kataKunci = txtCari.Text.Trim();
                    cmd.Parameters.AddWithValue("@cari", "%" + kataKunci + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Tampilkan hasil ke GridView
                    dgvLapangan.DataSource = dt;

                    // Jika hasil tidak ditemukan, berikan informasi
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Data tidak ditemukan.", "Pencarian");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan saat mencari: " + ex.Message);
                }
            }
        }

        // Bagian E: Fitur pilih data dari DataGridView ke TextBox
        private void dgvLapangan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLapangan.Rows[e.RowIndex];
                txtID.Text = row.Cells["LapanganID"].Value.ToString();
                txtNama.Text = row.Cells["NamaLapangan"].Value.ToString();
                txtLokasi.Text = row.Cells["Lokasi"].Value.ToString();
                cmbStatus.Text = row.Cells["Status"].Value.ToString();
            }
        }

        // Event handler lainnya dibiarkan kosong
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void lblTotal_Click(object sender, EventArgs e) { }
    }
}