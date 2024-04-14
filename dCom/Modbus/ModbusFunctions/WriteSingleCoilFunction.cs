using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusWriteCommandParameters param = CommandParameters as ModbusWriteCommandParameters;
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

            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.OutputAddress));
            request[8] = temp[0];
            request[9] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)param.Value));
            request[10] = temp[0];
            request[11] = temp[1];

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            Dictionary<Tuple<PointType, ushort>, ushort> dict = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ushort regNum = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, 8));
            ushort value = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, 10));
            Tuple<PointType, ushort> adress = new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, regNum);
            dict.Add(adress, value);
            return dict;
        }
    }
}