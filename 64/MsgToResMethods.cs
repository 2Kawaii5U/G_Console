using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hologic.CANRouter;
using Peak.Can.Light;

namespace ICLRead
{
    public class MsgToResMethods
    {
        #region Variables

        ConsoleForm console;

        MessageWriter writer;

        #endregion

        #region Constructor(s)

        public MsgToResMethods(ConsoleForm console)
        {
            this.console = console;

            this.writer = new MessageWriter(console);

        }

        #endregion

        #region BootLoader

        public void SendStartResponse(TCLightMsg msg)
        {
            //Start Send Seg
            ConsoleForm.RemoveMsgToResponse((uint)MSG.CAN_BTLD_SVCE_START);

            //foreach (TCLightMsg msg in BootLoader.ParsedMsgs)
            //{
            //    console.AddOutsideMessage(msg, writer.BuildMessageTime());
            //}

            console.AddMsgToResponse((uint)MSG.CAN_BLK_SEND_SEG, SendSegResponse);
            //writer.WriteMessage(BootLoader.GetMsg(), writer.BuildMessageTime());
        }

        public void SendSegResponse(TCLightMsg msg)
        {
            //remove the message
            ConsoleForm.RemoveMsgToResponse((uint)MSG.CAN_BLK_SEND_SEG);

            #region Declarations

            TCLightMsg currMsg;

            TCLightMsg DescMsg = writer.BuildMessage((uint)MSG.CAN_BLK_SEG_DESC, (uint)PID.CAN_PID_OTHER,
                                                     BootLoader.GetDID(),        (uint)SID.CAN_SID_EXT);

            bool finished = false;

            #endregion

            //Send the segment
            while (true)
            {
                currMsg = BootLoader.GetMsg();

                if (currMsg.Data[0] == currMsg.Data[1] && console.MsgEqual((uint)MSG.CAN_BLK_SEG_DATA, currMsg.ID))
                {
                    if (finished)
                    {
                        writer.WriteMessage(currMsg);
                        break;
                    }
                    else
                    {
                        console.AddMsgToResponse((uint)MSG.CAN_BLK_SEND_SEG, SendSegResponse);
                        writer.WriteMessage(currMsg);
                        break;
                    }
                }
                else
                {
                    writer.WriteMessage(currMsg);
                }
            }
        }

        #endregion

        #region GBI

        /// <summary>
        /// On Trigger, updates the GBI Page with values directly from the node
        /// </summary>
        /// <param name="msg">triggering message</param>
        public void ValNodeTestFunctionResponse(TCLightMsg msg)
        {
            if (msg.Data[0] == 8)//Not sure what this signifies, but it needs to be 8 for the GBI page
            {
                switch (msg.Data[1])//literally just a switch value
                {
                        //updates control items with values directly from the node
                    case 0:
                        console.GBICmd.IdleMv = BitConverter.ToUInt16(new byte[2] {msg.Data[3],msg.Data[2]}, 0);
                        console.GBICmd.StaticMv = BitConverter.ToUInt16(new byte[2] { msg.Data[5], msg.Data[4] }, 0);
                        console.GBICmd.type = msg.Data[6];
                        console.GBICmd.trigger = msg.Data[7];
                        console.GBICmd.UpdateAll();
                        break;
                    case 1:
                        console.GBICmd.startMv = BitConverter.ToUInt16(new byte[2] { msg.Data[3], msg.Data[2] }, 0);
                        console.GBICmd.stopMv = BitConverter.ToUInt16(new byte[2] { msg.Data[5], msg.Data[4] }, 0);
                        console.GBICmd.time = BitConverter.ToUInt16(new byte[2] { msg.Data[7], msg.Data[6] }, 0);
                        break;
                    case 2:
                        console.GBICmd.RampUpTime = msg.Data[2];
                        console.GBICmd.RampDownTime = msg.Data[3];
                        console.GBICmd.GBIScanStartDelay = BitConverter.ToUInt16(new byte[2] { msg.Data[5], msg.Data[4] }, 0);
                        console.GBICmd.GBIDetReadyDelay = BitConverter.ToUInt16(new byte[2] { msg.Data[7], msg.Data[6] }, 0);
                        break;
                    case 3:
                        console.GBICmd.StepAbsMv = BitConverter.ToUInt16(new byte[2] { msg.Data[3], msg.Data[2] }, 0);
                        console.GBICmd.StepIncMv = BitConverter.ToUInt16(new byte[2] { msg.Data[5], msg.Data[4] }, 0);
                        console.GBICmd.StepTime = BitConverter.ToUInt16(new byte[2] { msg.Data[7], msg.Data[6] }, 0);
                        console.GBICmd.UpdateAll();
                        break;
                    case 4:
                        console.GBICmd.DACOffsetMv = BitConverter.ToUInt16(new byte[2] { msg.Data[3], msg.Data[2] }, 0);
                        console.GBICmd.UpdateAll();
                        break;
                }
            }
        }

        #endregion
    }
}
