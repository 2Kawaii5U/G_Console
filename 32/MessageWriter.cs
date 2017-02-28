using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Peak.Can.Light;
using Hologic.CANRouter;

namespace ICLRead
{
    class MessageWriter
    {
        #region Private Variables

        /// <summary>
        /// Console to Write to
        /// </summary>
        private ConsoleForm console;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Constructor for MessageWriter Object
        /// </summary>
        /// <param name="console">Console to write to</param>
        public MessageWriter(ConsoleForm console)
        {
            this.console = console;
        }

        #endregion

        #region Generic

        /// <summary>
        /// Writes a Generic Message, check ActiveHardware
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="PID"></param>
        /// <param name="DID"></param>
        /// <param name="SID"></param>
        /// <param name="Data"></param>
        public void WriteMessage( uint ID, uint PID, uint DID, uint SID, Byte[] Data )
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            TCLightMsg newMsg = new TCLightMsg();
            TCLightTimestamp newTime = new TCLightTimestamp();

            newMsg.ID = ID | PID | DID | SID;

            newMsg.MsgType = MsgTypes.MSGTYPE_EXTENDED;

            newMsg.Len = 8;

            newMsg.Data = Data;

            newTime.millis = 1002;

            console.AddOutsideMessage(newMsg, newTime);
            PCANLight.Write(console.GetActiveHardware(), newMsg);
        }

        /// <summary>
        /// Writes a Generic Message
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="time"></param>
        public void WriteMessage(TCLightMsg msg, TCLightTimestamp time)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            console.AddOutsideMessage(msg, time);
            PCANLight.Write(console.GetActiveHardware(), msg);
        }

        public void WriteMessage(TCLightMsg msg)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            console.AddOutsideMessage(msg, BuildMessageTime());
            PCANLight.Write(console.GetActiveHardware(), msg);
        }

        public TCLightMsg BuildMessage( uint ID, uint PID, uint DID, uint SID)
        {
            TCLightMsg newMsg = new TCLightMsg();

            newMsg.ID = ID | PID | DID | SID;

            newMsg.MsgType = MsgTypes.MSGTYPE_EXTENDED;

            newMsg.Len = 8;

            return newMsg;
        }

        public TCLightTimestamp BuildMessageTime()
        {
            TCLightTimestamp time = new TCLightTimestamp();

            time.millis = (uint)DateTime.Now.Millisecond;
            return time;
        }

        #endregion

        #region Specific

        public TCLightMsg BuildCanBlkSendSeg()
        {
            TCLightMsg newMsg = new TCLightMsg();

            newMsg.ID = (uint)MSG.CAN_BLK_SEND_SEG | 
                        (uint)PID.CAN_PID_OTHER |
                        (uint)DID.CAN_DID_THD |
                        (uint)SID.CAN_SID_EXT;

            newMsg.MsgType = MsgTypes.MSGTYPE_EXTENDED;

            newMsg.Len = 8;

            return newMsg;
        }

        public TCLightMsg BuildCanBlkSendSegResponse()
        {
            TCLightMsg newMsg = new TCLightMsg();

            newMsg.ID = (uint)MSG.CAN_BLK_SEND_SEG |
                        (uint)PID.CAN_PID_OTHER |
                        (uint)DID.CAN_DID_EXT |
                        (uint)SID.CAN_SID_THD;

            newMsg.MsgType = MsgTypes.MSGTYPE_EXTENDED;

            newMsg.Len = 8;

            return newMsg;
        }

        #endregion
    }
}
