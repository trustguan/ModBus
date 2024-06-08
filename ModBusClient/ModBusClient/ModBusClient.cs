using Microsoft.Win32;
using NModbus;
using NModbus.Device;
using NModbus.Extensions.Enron;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NModbus.Serial;
namespace _ModBusClient
{
    enum ClientType { MODBUS_TCP,MODBUS_RTU}
    internal class ModBusClient
    {
        private ModbusFactory _modbusFactory;
        private IModbusMaster _modbusMaster;

        private TcpClient _tcpClient;
        private ClientType _clientType;
        private string _ip;
        private int _port;

        private SerialPort _serialClient;
        private string _portName;
        private int _boundRate;
        private int _dataBits;
        private Parity _parity;
        private StopBits _stopBits;
        public ModBusClient(string ip,int port) 
        {
             _modbusFactory= new ModbusFactory();
              _ip = ip;
              _port = port;
             _clientType = ClientType.MODBUS_TCP;
             _tcpClient = new TcpClient();
            _modbusMaster = null;
        }

        public ModBusClient(string com, int boundRate, int dataBits,Parity parity,StopBits stopBits) 
        { 
            _serialClient = new SerialPort();
            _portName = com;
            _boundRate = boundRate;
            _dataBits = dataBits;
            _parity = parity;
            _stopBits = stopBits;

            _serialClient.PortName = _portName;
            _serialClient.BaudRate = _boundRate;
            _serialClient.Parity = _parity;
            _serialClient.DataBits = _dataBits;
            _serialClient.StopBits = _stopBits;

            _clientType = ClientType.MODBUS_RTU;
            _modbusFactory = new ModbusFactory();
            _modbusMaster= null;
        }

        public static string[] GetPortNames()
        { 
            return SerialPort.GetPortNames();
        }
        public bool Connect() {

            if (_clientType == ClientType.MODBUS_TCP)
            {
                try
                {
                    _tcpClient.Connect(_ip, _port);
                    _modbusMaster = _modbusFactory.CreateMaster(_tcpClient);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else 
            {
                try
                {
                    _serialClient.Open();
                    _modbusMaster = _modbusFactory.CreateRtuMaster(_serialClient);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public void Disconnect()
        {
            if (_clientType == ClientType.MODBUS_TCP)
            {
                _tcpClient.Close();
            }
            else { 
                _serialClient.Close();
            }
        }

        public bool ReadFloatValues(int address, int nums, out List<float> results, int slaveId = 1, bool isBigEndian=true, bool swapBytes=false)
        {
            List<float> vals = new List<float>();
            results = vals;
            if (_modbusMaster != null)
            {
                try
                {
                    for (int i = 0; i < nums; i++,address+=2) {
                        ushort[] registers = _modbusMaster.ReadHoldingRegisters((byte)slaveId, (ushort)address, 2);
                        results.Add(GetFloat(registers[0], registers[1],isBigEndian,swapBytes));
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        public bool WriteFloatValues(int address, List<float> valus, int slaveId = 1, bool isBigEndian = true, bool swapBytes = false)
        {
            if (_modbusMaster != null)
            {
                try
                {
                    ushort[] registers=new ushort[valus.Count*2];
                   for (int i = 0;i < valus.Count;i++)
                    {
                        ushort[] tmp = GetUshorts(valus[i], isBigEndian, swapBytes);
                        registers[2*i] = tmp[0];
                        registers[2*i+1] = tmp[1];
                        
                    }
                    _modbusMaster.WriteMultipleRegisters((byte)slaveId, (ushort)address, registers);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public bool ReadDoubleValues(int address, int nums, out List<double> results, int slaveId = 1, bool isBigEndian = true, bool swapBytes = false)
        {
            List<double> vals = new List<double>();
            results = vals;
            if (_modbusMaster != null)
            {
                try
                {
                    for (int i = 0; i < nums; i++, address += 4)
                    {
                        ushort[] registers = _modbusMaster.ReadHoldingRegisters((byte)slaveId, (ushort)address, 4);
                        results.Add(GetDouble(registers[0], registers[1], registers[2], registers[3], isBigEndian, swapBytes));
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        public bool WriteDoubleValues(int address, List<double> valus, int slaveId = 1, bool isBigEndian = true, bool swapBytes = false)
        {
            if ( _modbusMaster != null)
            {
                try
                {
                    ushort[] registers = new ushort[valus.Count * 4];
                    for (int i = 0; i < valus.Count; i++)
                    {
                        ushort[] tmp = GetUshorts(valus[i], isBigEndian, swapBytes);
                        registers[2 * i] = tmp[0];
                        registers[2 * i + 1] = tmp[1];
                        registers[2 * i + 2] = tmp[2];
                        registers[2 * i + 3] = tmp[3];

                    }
                    _modbusMaster.WriteMultipleRegisters((byte)slaveId, (ushort)address, registers);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public bool ReadUintValues(int address, int nums,out List<ushort> results, int slaveId = 1) 
        { 
            List<ushort> vals = new List<ushort>();
            results = vals;
            if (_modbusMaster!=null)
            {
                try {
                    ushort[] registers = _modbusMaster.ReadHoldingRegisters((byte)slaveId, (ushort)address, (ushort)nums);
                    for (int i = 0; i < nums; i++)
                    {
                        vals.Add(registers[i]);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }


        public bool WriteUintValues(int address,List<ushort>valus,int slaveId=1) 
        {
            if ( _modbusMaster != null)
            {
                try
                {
                    ushort[] v = new ushort[valus.Count];
                    for (int i = 0; i < valus.Count; i++) {
                        v[i] = valus[i];
                    }
                    _modbusMaster.WriteMultipleRegisters((byte)slaveId, (ushort)address,v);
                    return true;
       
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public bool ReadIOs(int address, int nums, out List<bool> results, int slaveId = 1)
        {
            List<bool> vals = new List<bool>();
            results = vals;
            if (_modbusMaster != null)
            {
                try
                {
                    bool[] cols = _modbusMaster.ReadCoils((byte)slaveId, (ushort)address, (ushort)nums);
                    for (int i = 0; i < nums; i++)
                    {
                        vals.Add(cols[i]);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        public bool WriteIOs(int address, List<bool> valus, int slaveId = 1)
        {
            if (_modbusMaster != null)
            {
                try
                {
                    bool[] v = new bool[valus.Count];
                    for (int i = 0; i < valus.Count; i++)
                    {
                        v[i] = valus[i];
                    }
                    _modbusMaster.WriteMultipleCoils((byte)slaveId, (ushort)address, v);
                    return true;

                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public static float GetFloat(ushort P1, ushort P2, bool isBigEndian, bool swapBytes)
        {
            if (swapBytes)
            {
                P1 = SwapBytes(P1);
                P2 = SwapBytes(P2);
            }

            uint combined = (uint)(P1 << 16) | P2;
            if (BitConverter.IsLittleEndian != isBigEndian)
            {
                combined = ReverseEndianness(combined);
            }

            byte[] bytes = BitConverter.GetBytes(combined);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double GetDouble(ushort P1, ushort P2, ushort P3, ushort P4, bool isBigEndian, bool swapBytes)
        {
            if (swapBytes)
            {
                P1 = SwapBytes(P1);
                P2 = SwapBytes(P2);
                P3 = SwapBytes(P3);
                P4 = SwapBytes(P4);
            }

            ulong combined = ((ulong)P1 << 48) | ((ulong)P2 << 32) | ((ulong)P3 << 16) | P4;
            if (BitConverter.IsLittleEndian != isBigEndian)
            {
                combined = ReverseEndianness(combined);
            }

            byte[] bytes = BitConverter.GetBytes(combined);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static ushort[] GetUshorts(float value, bool isBigEndian, bool swapBytes)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian != isBigEndian)
            {
                Array.Reverse(bytes);
            }
            uint combined = BitConverter.ToUInt32(bytes, 0);
            ushort P1 = (ushort)(combined >> 16);
            ushort P2 = (ushort)(combined & 0xFFFF);
            if (swapBytes)
            {
                P1 = SwapBytes(P1);
                P2 = SwapBytes(P2);
            }
            return new ushort[] { P1, P2 };
        }

        public static ushort[] GetUshorts(double value, bool isBigEndian, bool swapBytes)
        {
            // 将double值转换为字节数组
            byte[] bytes = BitConverter.GetBytes(value);

            // 判断系统字节序是否与要求一致，如果不一致则反转字节数组
            if (BitConverter.IsLittleEndian != isBigEndian)
            {
                Array.Reverse(bytes);
            }

            // 将字节数组转换为64位无符号整数
            ulong combined = BitConverter.ToUInt64(bytes, 0);

            // 分别提取4个ushort
            ushort P1 = (ushort)(combined >> 48);
            ushort P2 = (ushort)((combined >> 32) & 0xFFFF);
            ushort P3 = (ushort)((combined >> 16) & 0xFFFF);
            ushort P4 = (ushort)(combined & 0xFFFF);

            // 如果swapBytes为true，则交换每个ushort的字节顺序
            if (swapBytes)
            {
                P1 = SwapBytes(P1);
                P2 = SwapBytes(P2);
                P3 = SwapBytes(P3);
                P4 = SwapBytes(P4);
            }

            // 返回包含4个ushort的数组
            return new ushort[] { P1, P2, P3, P4 };
        }

        private static ushort SwapBytes(ushort value)
        {
            return (ushort)((value << 8) | (value >> 8));
        }

        private static uint ReverseEndianness(uint value)
        {
            return ((value & 0x000000FF) << 24) |
                   ((value & 0x0000FF00) << 8) |
                   ((value & 0x00FF0000) >> 8) |
                   ((value & 0xFF000000) >> 24);
        }
        private static ulong ReverseEndianness(ulong value)
        {
            return ((value & 0x00000000000000FFUL) << 56) |
                   ((value & 0x000000000000FF00UL) << 40) |
                   ((value & 0x0000000000FF0000UL) << 24) |
                   ((value & 0x00000000FF000000UL) << 8) |
                   ((value & 0x000000FF00000000UL) >> 8) |
                   ((value & 0x0000FF0000000000UL) >> 24) |
                   ((value & 0x00FF000000000000UL) >> 40) |
                   ((value & 0xFF00000000000000UL) >> 56);
        }

    }
}
