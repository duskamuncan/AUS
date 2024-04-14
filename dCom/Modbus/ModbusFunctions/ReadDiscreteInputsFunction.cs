using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read discrete inputs functions/requests.
    /// </summary>
    public class ReadDiscreteInputsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadDiscreteInputsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadDiscreteInputsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
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
            ushort bitCount = param.Quantity;

            //reading 
            for (int i = 0; i < byteCount; i++)
            {
                byte temp = response[9 + i];
                for (int j = 0; j < 8; j++)
                {
                    if (j + (ushort)(i) * 8 == bitCount)
                    {
                        break;
                    }
                    Tuple<PointType, ushort> adress = new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, startAdress++);
                    ushort value = (ushort)((temp & (1 << j)) != 0 ? 1 : 0);
                    dict.Add(adress, value);
                }
            }

            return dict;
        }
    }
}