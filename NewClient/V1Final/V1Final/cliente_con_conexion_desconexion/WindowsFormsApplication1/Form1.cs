using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Libreria;
using System.Threading;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        Funciones funciones = new Funciones();
        string Nombre;
        int puerto = 50085;
       
       
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false; //Necesario para que los elementos de los formularios puedan ser
            //accedidos desde threads difetentes a los que los crearon
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            groupBox1.Visible = false;
            button3.Visible = false;
            //Servicios.Visible = false;
            contSer.Visible = false;
            dataGridView2.Visible = false;
            //dataGridView1.RowCount = 1;
            dataGridView2.ColumnCount = 1;
            dataGridView2.Columns[0].HeaderCell.Value = "Usuarios Conectados";
            dataGridView2.AutoSize = true;






        }

         private void AtenderServidor()
        {
            //try
            //{

                while (true)
                {
                    //Recibimos mensaje del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                    int codigo = Convert.ToInt32(trozos[0]);
                    string mensaje = mensaje = trozos[1].Split('\0')[0];

                    switch (codigo)
                    {
                        case 1: //respuesta C1
                            //string mensaje = funciones.ConQuienJuego(ID.Text, server);

                            MessageBox.Show("Has jugado con: " + mensaje);
                            break;
                        case 2: //respuesta C2

                            MessageBox.Show("El jugador con más puntos es: " + mensaje);
                            break;
                        case 3:  //respuesta C3

                            MessageBox.Show(mensaje);
                            break;
                        case 6: //ListaConectados

                            //string[] trozos1 = mensaje.Split('/');
                            int i = 1;
                            //dataGridView1.RowCount = Convert.ToInt32(trozos[1]) + 1;

                            while (i <= Convert.ToInt32(trozos[1]))
                            {
                                dataGridView2.Rows.Add(trozos[i + 1]);
                                //dataGridView1.Rows[i].Cells[0].Value = trozos[i+1];
                                i = i + 1;

                            }
                            //MessageBox.Show("He recibido la Lista de Conectados");
                            
                            break;
                        case 8: //Recibimos notificación

                            contSer.Text = mensaje;
                            break;


                    }
                }
            //}
            //catch (FormatException t)
            //{
            //    MessageBox.Show("Error: " + t.Message);
            //}

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                if (ConQuienJuego.Checked)
                {
                    //Consulta 1
                    // Busca los jugadores con los que ha jugado la persona que introduces por consola

                    string mensaje = "1/" + ID.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    //string mensaje = funciones.ConQuienJuego(ID.Text, server);
                    //MessageBox.Show("Has jugado con: " + mensaje);
                }
                else if (MasPuntos.Checked)
                {
                    //Consulta 2
                    //Busca que jugador que ha jugado en la partida introducida por consola tiene mas puntos acumulados
                    string mensaje = "2/" + partida.Text;
                    // Enviamos al servidor el ID de la partida tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    //string mensaje = funciones.QuienTieneMasPuntos(partida.Text, server);
                    //MessageBox.Show("El jugador con más puntos es: " + mensaje);

                }
                else
                {
                    //Consulta 3
                    // Busca la partida mas corta y que su tiempo es inferior a 1500s
                    string mensaje = "3/ ";
                    // En este caso no es necesario introducir ningún dato extra
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    //string mensaje = funciones.PartidaMasCorta(server);
                    //MessageBox.Show(mensaje);
                }
            }
            catch (FormatException t)
            {
                MessageBox.Show("Hay un error: " + t.Message);
            }
                

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "7/" + Nombre;

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            atender.Abort();
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            
            
            //funciones.Desconnectar(server);
            this.BackColor = Color.Gray;
            groupBox1.Visible = false;
            groupBox2.Visible = true;
            button3.Visible = false;
            //Servicios.Visible = false;
            contSer.Visible = false;
            dataGridView2.Visible = false;
  

        }

        private void Login_Click(object sender, EventArgs e)
        {
            //Log in
            //Nos permite comprobar que el usuario y la contraseña introducidos existen en la base de datos
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("147.83.117.22");
            IPEndPoint ipep = new IPEndPoint(direc, puerto);



                //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
            //Boolean conectado = funciones.Connectar();
            try
                {
                    server.Connect(ipep);//Intentamos conectar el socket
                    this.BackColor = Color.Green;
                    MessageBox.Show("Conectado");
                    //Enviamos al servidor el nombre y la contraseña del usuario existente
                    string mensaje = "4/" + UsuariLogIn.Text + "/" + contrasenyaLogIn.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    //string mensaje = funciones.LogIn(UsuariLogIn.Text, contrasenyaLogIn.Text, server);
                    
                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                    MessageBox.Show("Este es el mensaje:" + mensaje);
                    //Nos encontramos con 3 casos diferentes
                    if (mensaje == "NO EXISTE")
                        MessageBox.Show("Este usuario no existe, por favor regístrate");
                    else if (mensaje == "LLENO")
                        MessageBox.Show("No podemos atenderte en este momento. Inténtalo más tarde");
                    else if (mensaje == "OK")
                    {
                        MessageBox.Show("Bienvenido a tu cuenta :)");
                        groupBox1.Visible = true;
                        button3.Visible = true;
                        groupBox2.Visible = false;
                        dataGridView2.Visible = true;
                        //Pongo en marcha el thread que atenderá los mensajes del servidor
                        ThreadStart ts = delegate { AtenderServidor(); };
                        atender = new Thread(ts);
                        atender.Start();

                        //Servicios.Visible = true;
                        contSer.Visible = true;
                        Nombre = UsuariLogIn.Text;
                    }

                    else
                        MessageBox.Show("La contraseña introducida es incorrecta");

                }
                catch (SocketException ex)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;

                }
            

            

            
        }

        private void SignIn_Click(object sender, EventArgs e)
        {
            //Sign in

            
                //Nos permite añadir un nuevo usuario en la base de datos
            IPAddress direc = IPAddress.Parse("147.83.117.22");
            IPEndPoint ipep = new IPEndPoint(direc, puerto);

                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Boolean conectado = funciones.Connectar();
                try
                {
                    server.Connect(ipep);//Intentamos conectar el socket
                    this.BackColor = Color.Green;
                    MessageBox.Show("Conectado");

                    //Enviamos al servidor el nombre y la contraseña del usuario nuevo
                    string mensaje = "5/" + UsuariSignIn.Text + "/" + ContrasenyaSignIn.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    //string mensaje = funciones.SignIn(UsuariSignIn.Text, ContrasenyaSignIn.Text, server);
                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];



                    //Nos encontramos con 3 casos
                    if (mensaje == "ERROR")
                        MessageBox.Show("Ha habido un error, revisa bien los campos");
                    else if (mensaje == "OK")
                    {
                        MessageBox.Show("Registrado correctamente! Bienvenido :)");
                       // groupBox1.Visible = true;

                        
                    }
                    else
                        MessageBox.Show("Lo sentimos, ese usuario ya existe");
                }
                catch (SocketException ex)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;

                }
                //Mensaje de desconexión
                string mensaje2 = "0/";

                byte[] msg3 = System.Text.Encoding.ASCII.GetBytes(mensaje2);
                server.Send(msg3);

                // Nos desconectamos
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();


                //funciones.Desconnectar(server);
                this.BackColor = Color.Gray;
                groupBox1.Visible = false;
                groupBox2.Visible = true;
                //Servicios.Visible = false;
                contSer.Visible = false;
                dataGridView2.Visible = false;
            
            

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void UsuariLogIn_TextChanged(object sender, EventArgs e)
        {

        }



        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void Usuario_Click(object sender, EventArgs e)
        {

        }



        private void Enter_Click(object sender, EventArgs e)
        {
            MapaJuego form = new MapaJuego();
            form.ShowDialog();
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void contSer_Click(object sender, EventArgs e)
        {

        }

       

       
    }
}
