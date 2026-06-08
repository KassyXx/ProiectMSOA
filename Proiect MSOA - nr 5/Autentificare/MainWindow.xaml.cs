using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Proiect_MSOA___nr_5
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new BazaDeDateContext())
                {
                    // 1. Inserare Utilizatori 
                    if (!db.Utilizatori.Any(u => u.Username == "admin"))
                    {
                        db.Utilizatori.Add(new Utilizator { Username = "admin", Parola = "admin123", Rol = "Administrator" });
                        db.Utilizatori.Add(new Utilizator { Username = "ionut", Parola = "ionut123", Rol = "Angajat" });
                        db.Utilizatori.Add(new Utilizator { Username = "andrei", Parola = "andrei123", Rol = "Client" });
                        db.SaveChanges();
                    }

                    // 2. Inserare Produse si Servicii de test
                    if (!db.ProduseServicii.Any())
                    {
                        db.ProduseServicii.Add(new ProdusServiciu { Denumire = "Tuns Clasic + Spălat", Pret = 50, Tip = "Serviciu" });
                        db.ProduseServicii.Add(new ProdusServiciu { Denumire = "Tuns Barbă + Contur", Pret = 30, Tip = "Serviciu" });
                        db.ProduseServicii.Add(new ProdusServiciu { Denumire = "Pachet Full (Tuns + Barbă)", Pret = 75, Tip = "Serviciu" });
                        db.ProduseServicii.Add(new ProdusServiciu { Denumire = "Ceară de păr Premium", Pret = 45, Tip = "Produs" });
                        db.ProduseServicii.Add(new ProdusServiciu { Denumire = "Ulei de barbă Organic", Pret = 60, Tip = "Produs" });
                        db.SaveChanges();
                    }

                    // 3. INSERARE EXTINSA: Programari istorice si viitoare pentru rapoarte financiare reale
                    if (!db.Programari.Any())
                    {
      
                        var clientId = db.Utilizatori.Where(u => u.Rol == "Client").Select(u => u.Id).FirstOrDefault();
                        var angajatId = db.Utilizatori.Where(u => u.Rol == "Angajat").Select(u => u.Id).FirstOrDefault();

                   
                        if (clientId == 0) clientId = 3;
                        if (angajatId == 0) angajatId = 2;

                    
                        db.Programari.Add(new Programare { IdClient = clientId, IdAngajat = angajatId, IdServiciu = 1, DataOra = DateTime.Now.AddDays(-2), Status = "Finalizată & Plătită" });
                        db.Programari.Add(new Programare { IdClient = clientId, IdAngajat = angajatId, IdServiciu = 2, DataOra = DateTime.Now.AddDays(-1), Status = "Finalizată & Plătită" });
                        db.Programari.Add(new Programare { IdClient = clientId, IdAngajat = angajatId, IdServiciu = 3, DataOra = DateTime.Now.AddDays(-1), Status = "Finalizată & Plătită" });
                        db.Programari.Add(new Programare { IdClient = clientId, IdAngajat = angajatId, IdServiciu = 3, DataOra = DateTime.Now.AddDays(1), Status = "Confirmată" });
                        db.Programari.Add(new Programare { IdClient = clientId, IdAngajat = angajatId, IdServiciu = 1, DataOra = DateTime.Now.AddDays(2), Status = "În așteptare" });

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la popularea automată a bazei de date: {ex.Message}", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

    
            // PASUL 2: PROCESUL CLASIC DE LOGARE
    
            string usernameIntrodus = txtUsername.Text;
            string parolaIntrodusa = txtParola.Password;

            if (string.IsNullOrEmpty(usernameIntrodus) || string.IsNullOrEmpty(parolaIntrodusa))
            {
                MessageBox.Show("Vă rugăm să completați toate câmpurile!", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new BazaDeDateContext())
                {
                    var utilizatorGasit = db.Utilizatori.FirstOrDefault(u => u.Username == usernameIntrodus && u.Parola == parolaIntrodusa);

                    if (utilizatorGasit != null)
                    {
                        string rol = utilizatorGasit.Rol.ToLower();

                        if (rol == "administrator" || rol == "admin")
                        {
                            MessageBox.Show("Autentificare reușită ca Administrator!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                            AdminWindow adminWin = new AdminWindow();
                            adminWin.Show();
                            this.Close();
                        }
                        else if (rol == "client")
                        {
                            MessageBox.Show("Autentificare reușită! Bine ai venit la Barber Shop.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                            ClientWindow clientWin = new ClientWindow();
                            clientWin.Show();
                            this.Close();
                        }
                        else if (rol == "angajat" || rol == "staff")
                        {
                            MessageBox.Show("Autentificare reușită ca Staff! Se încarcă panoul zilnic.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                            EmployeeWindow employeeWin = new EmployeeWindow();
                            employeeWin.Show(); // Deschide portalul angajatului
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Rol necunoscut în sistem!", "Eroare");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Utilizator sau parolă incorectă!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la conectarea cu baza de date: {ex.Message}", "Eroare critică", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}