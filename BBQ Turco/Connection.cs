﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BBQ_Turco
{
    static class Connection
    {

        static public string message_sent;
        static public string message_received;
        static public bool connection = false;
        static public bool is_connection_broken = false;
        static public bool is_return_answer = false;
        static public bool is_process_continue = false;
        static public Queue<string> UDP_commands = new Queue<string>();
        static public void SendNewMessage(string msg)
        {
            if (is_process_continue)
            {
                while (is_process_continue) ;
            }
            message_sent = msg;
            is_process_continue = true;
            if (is_process_continue)
            {
                while (is_process_continue) ;
            }
        }
        static public void UDPListener()
        {
            UdpClient listener = new UdpClient();
            listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 11000);
            try
            {
                listener.Client.Bind(endPoint);
                Console.WriteLine("Connected to the UDP server.");
                while (true)
                {
                    byte[] bytes = listener.Receive(ref endPoint);
                    string message_broadcast = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    Console.WriteLine($"Received broadcast from {endPoint} : {message_broadcast}\n");
                    UDP_commands.Enqueue(message_broadcast);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("UDP Connection Error.");
                Console.WriteLine(e.Message);
            }
            finally
            {
                listener.Close();
            }
        }
        static public void TCPConnection()
        {
            while (true)
            {
                try
                {
                    TcpClient tcpClient = new TcpClient("127.0.0.1", 1234);
                    StreamWriter sWriter = new StreamWriter(tcpClient.GetStream());
                    Console.WriteLine("Connected to the TCP server." + "\n");
                    Thread readThread = new Thread(TCPListener);
                    readThread.Start(tcpClient);
                    connection = true;
                    if (is_connection_broken)
                    {
                        MessageBox.Show("The connection has been restored.");
                        is_connection_broken = false;
                    }
                    while (true)
                    {
                        if (tcpClient.Connected)
                        {
                            if (is_process_continue)
                            {
                                sWriter.WriteLine(message_sent);
                                sWriter.Flush();
                                is_return_answer = false;
                                if (!is_return_answer)
                                {
                                    while (!is_return_answer) ;
                                }
                                Console.WriteLine("Sent message: " + message_sent);
                                Console.WriteLine("Received message: " + message_received + "\n");
                                is_process_continue = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("The connection is broken. Please check the connection.");
                            connection = false;
                            is_connection_broken = true;
                            Thread.Sleep(1000);
                            break;
                        }
                    }

                }
                catch (Exception e)
                {
                    connection = false;
                    MessageBox.Show(e.Message);
                    Thread.Sleep(1000);
                }
            }
        }
        static public void TCPListener(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;
            StreamReader sReader = new StreamReader(tcpClient.GetStream());
            while (true)
            {
                try
                {
                    message_received = sReader.ReadLine();
                    is_return_answer = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }
        }
        static public bool CheckIfFormIsOpen(string form_name)
        {
            FormCollection formCollection = Application.OpenForms;
            foreach (Form form in formCollection)
            {
                if (form.Name == form_name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
