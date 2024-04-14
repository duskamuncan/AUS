using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters param = this.CommandParameters as ModbusReadCommandParameters;
            byte[] request = new byte[12];
            byte[] temp;
            // packing
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.TransactionId));
            request[0] = temp[0];
            request[1] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.ProtocolId));
            request[2] = temp[0];
            request[3] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.Length));
            request[4] = temp[0];
            request[5] = temp[1];

            //unitid and functioncode already bytes
            request[6] = param.UnitId;
            request[7] = param.FunctionCode;

            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.StartAddress));
            request[8] = temp[0];
            request[9] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.Quantity));
            request[10] = temp[0];
            request[11] = temp[1];

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            Dictionary<Tuple<PointType, ushort>, ushort> dict = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusReadCommandParameters param = CommandParameters as ModbusReadCommandParameters;
            ushort startAdress = param.StartAddress;
            byte byteCount = response[8];
            ushort value;
            //reading 
            for (int i = 0; i < byteCount; i += 2)
            {
                Tuple<PointType, ushort> adress = new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, startAdress++);
                value = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, 9 + i));
                dict.Add(adress, value);
            }

            return dict;
        }
    }
}