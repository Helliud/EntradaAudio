﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Dsp;
using System.Windows.Threading;
using System.Diagnostics;
using System.Media;

namespace Entrada
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaveIn waveIn;
        DispatcherTimer timer;
        float frecuenciaFundamental;
        Stopwatch cronometro;
        Stopwatch cronometroIniciar;
        string Grave = "Grave";
        string Agudo = "Agudo";
        string ganadorPlayer = "GANASTE";
        string perdedor = "PERDISTE";
        Random numeroRandom = new Random();
        int numeroInstruccion;
        int voz;
        float velocidadJugador;
        float velocidadBot;
        float nivelFrecuencia;
        int tiempoCambioInstruccion;
        bool comenzarJuego;
        SoundPlayer Player;
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            cronometro = new Stopwatch();
            cronometroIniciar = new Stopwatch();

            LlenarComboDispositivos();
            timer.Interval = TimeSpan.FromMilliseconds(100);

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(cronometroIniciar.ElapsedMilliseconds < 1000)
            {
                imgSemaforoCuerpo.Visibility = Visibility.Visible;
                imgSemaforoRojo.Visibility = Visibility.Visible;

            }
            if (cronometroIniciar.ElapsedMilliseconds >= 1300)
            {
                imgSemaforoAmarillo.Visibility = Visibility.Visible;

            }

            if (cronometroIniciar.ElapsedMilliseconds >= 2300)
            {
                imgSemaforoVerde.Visibility = Visibility.Visible;

            }

            if (cronometroIniciar.ElapsedMilliseconds >= 3200)
            {
                imgSemaforoVerde.Visibility = Visibility.Visible;
                imgSemaforoVerde2.Visibility = Visibility.Visible;
                imgSemaforoVerde3.Visibility = Visibility.Visible;
            }

            if (cronometroIniciar.ElapsedMilliseconds >= 4000)
            {
                imgSemaforoCuerpo.Visibility = Visibility.Hidden;
                imgSemaforoRojo.Visibility = Visibility.Hidden;
                imgSemaforoAmarillo.Visibility = Visibility.Hidden;
                imgSemaforoVerde.Visibility = Visibility.Hidden;
                imgSemaforoVerde2.Visibility = Visibility.Hidden;
                imgSemaforoVerde3.Visibility = Visibility.Hidden;
            }





            if (cronometroIniciar.ElapsedMilliseconds > 4500)
            {
                btnDetener.IsEnabled = true;
                avanzarBot();

                if (frecuenciaFundamental < nivelFrecuencia)
                {
                    voz = 0;
                }
                else if (frecuenciaFundamental > nivelFrecuencia)
                {
                    voz = 1;
                }

                etiquetas();
                marcadorCorrecto();

                if (numeroInstruccion == voz && frecuenciaFundamental < 1200 && frecuenciaFundamental > 50)
                {
                    avanzarJugador();
                }
                else if (numeroInstruccion != voz)
                {
                    lblCorrecto.Text = "MAL";
                    lblCorrecto.Foreground = new SolidColorBrush(Colors.Red);

                }


                if (cronometro.ElapsedMilliseconds >= tiempoCambioInstruccion)
                {
                    cronometro.Restart();
                    cambioInstruccion();
                }

                ganadorJugador();


            }

        }

        public void LlenarComboDispositivos()
        {
            for(int i = 0; i < WaveIn.DeviceCount; i++)
            {
                WaveInCapabilities capacidades = WaveIn.GetCapabilities(i);
                cmbDispositivos.Items.Add(capacidades.ProductName);
            }
            cmbDispositivos.SelectedIndex = 0;
            
        }


        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            Player = new SoundPlayer(@"D:\Users\MSI GL63\Source\Repos\EntradaAudio\Entrada\semaforo.wav");
            Player.Play();

            cronometro.Start();
            cronometroIniciar.Start();
            timer.Start();
            waveIn = new WaveIn();
            //Formato de audio
            waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            //Buffer
            waveIn.BufferMilliseconds = 250;
            //¿Que hacer cuando hay muestras disponibles?
            waveIn.DataAvailable += WaveIn_DataAvailable;

            waveIn.StartRecording();

            if (cmdFacil.IsSelected)
            {
                tiempoCambioInstruccion = 2000;
                velocidadBot = 0.8f;
                nivelFrecuencia = 400.0f;
                velocidadJugador = 20;
                lblFecuenciaDificultad.Text = nivelFrecuencia.ToString();
                lblFecuenciaDificultad2.Text = nivelFrecuencia.ToString();

            }

            else if (cmdMedio.IsSelected)
            {
                velocidadBot = 0.7f;
                velocidadJugador = 13;
                nivelFrecuencia = 500.0f;
                tiempoCambioInstruccion = 1000;
                lblFecuenciaDificultad.Text = nivelFrecuencia.ToString();
                lblFecuenciaDificultad2.Text = nivelFrecuencia.ToString();

            }

            else if (cmdDificl.IsSelected)
            {
                velocidadBot = 0.7f;
                velocidadJugador = 10;
                nivelFrecuencia = 650.0f;
                tiempoCambioInstruccion = 700;
                lblFecuenciaDificultad.Text = nivelFrecuencia.ToString();
                lblFecuenciaDificultad2.Text = nivelFrecuencia.ToString();


            }
        }

        void marcadorCorrecto()
        {
            if (frecuenciaFundamental < 1200)
            {
                lblCorrecto.Text = "Correcto";
                lblCorrecto.Foreground = new SolidColorBrush(Colors.GreenYellow);

            }
            else if (frecuenciaFundamental > 1200)
            {
                lblCorrecto.Text = "Baja tu voz";
                lblCorrecto.Foreground = new SolidColorBrush(Colors.Red);
            }

            else if (frecuenciaFundamental < 50)
            {
                lblCorrecto.Text = "Sube tu voz";
                lblCorrecto.Foreground = new SolidColorBrush(Colors.Red);

            }
            



        }

        void cambioInstruccion()
        {
            numeroInstruccion = numeroRandom.Next(0, 2);
        }

        void avanzarBot()
        {
            var leftCarro = Canvas.GetLeft(imgCarro);
            Canvas.SetLeft(imgCarro, leftCarro + (10.0f * velocidadBot));

        }

        void avanzarJugador()
        {
            var leftPlayer = Canvas.GetLeft(imgPlayer);
            Canvas.SetLeft(imgPlayer, leftPlayer + (frecuenciaFundamental / 500.0) * velocidadJugador);
        }

        void ganadorJugador()
        {
            //Ganar Jugador
            if (Canvas.GetLeft(imgPlayer) >= 1300)
            {
                waveIn.StopRecording();
                timer.Stop();
                lblGanador.Text = ganadorPlayer.ToString();
                lblCorrecto.Text = "";
            }

            if (Canvas.GetLeft(imgCarro) >= 1300)
            {
                waveIn.StopRecording();
                timer.Stop();
                lblGanador.Text = perdedor.ToString();
                lblCorrecto.Text = "";

            }
        }

        void etiquetas()
        {
            if (numeroInstruccion == 0)
            {
                lblNumRandom.Text = Grave.ToString();
            }

            else
            {
                lblNumRandom.Text = Agudo.ToString();
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            float acumulador = 0.0f;
            int bytesGrabados = e.BytesRecorded;
            double numeroDeMuestras = bytesGrabados / 2;
            int exponente = 1;
            int NumeroDeMuestrasComplejas = 0;
            int bitsMaximos = 0;

            do
            {
                bitsMaximos = (int)Math.Pow(2, exponente);
                exponente++;

            } while (bitsMaximos < numeroDeMuestras);

            NumeroDeMuestrasComplejas = bitsMaximos / 2;
            exponente-=2;

            Complex[] senalCompleja = new Complex[NumeroDeMuestrasComplejas];

            for (int i = 0; i < bytesGrabados; i += 2)
            {
                //Transformando 2 bytes separados en una muestra de 16 bits
                //1.- Toma el suegundo byte y el antepone 8 0's al principio
                //2.- Hace un OR con el primer byte, al cual automaticamente se le llenan 8 0's al final
                short muestra = (short)(buffer[i + 1] << 8 | buffer [i]);
                float muestra32bits = (float)muestra / 32768.0f;
                acumulador += Math.Abs(muestra32bits);

                if(i/2 < NumeroDeMuestrasComplejas)
                {
                    senalCompleja[i / 2].X = muestra32bits;
                }
            }
            float promedio = acumulador / (bytesGrabados / 2.0f);


            //FastFourierTransform.FFT();

            if(promedio > 0)
            {
                FastFourierTransform.FFT(true, exponente, senalCompleja);

                float[] valoresAbsolutos = new float[senalCompleja.Length];
                for(int i=0; i<senalCompleja.Length; i++)
                {
                    valoresAbsolutos[i] = (float)Math.Sqrt((senalCompleja[i].X * senalCompleja[i].X) + (senalCompleja[i].Y * senalCompleja[i].Y));
                }

                int indiceSenalConMasPresencia = valoresAbsolutos.ToList().IndexOf(valoresAbsolutos.Max());

                frecuenciaFundamental = (float)indiceSenalConMasPresencia * waveIn.WaveFormat.SampleRate / (float)valoresAbsolutos.Length;
                lblFrecuencia.Text = frecuenciaFundamental.ToString("f");
                

                

            }
        }

        private void BtnDetener_Click(object sender, RoutedEventArgs e)
        {
            waveIn.StopRecording();
            timer.Stop();
            cronometro.Reset();
            cronometroIniciar.Reset();
            Canvas.SetLeft(imgCarro, 0);
            Canvas.SetLeft(imgPlayer, 0);
            lblCorrecto.Text = "";
            lblGanador.Text = "";
            lblFrecuencia.Text = "";
            lblNumRandom.Text = "";
            lblFecuenciaDificultad.Text = "";
            lblFecuenciaDificultad2.Text = "";
            btnDetener.IsEnabled = false;

        }

    }
}
