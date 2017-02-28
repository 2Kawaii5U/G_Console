using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Peak.Can.Light;

namespace ICLRead
{
    public partial class MMICommandForm : Form
    {
        #region Private Variables

        private HardwareType ActiveHardware;

        private ConsoleForm console;

        private List<String> cmds = null;

        private byte numPackets = 0;

        #endregion

        #region Constructors
        /// <summary>
        /// Creates Form without Active Hardware
        /// </summary>
        public MMICommandForm(ConsoleForm console)
        {
            InitializeComponent();
            // add active hardware somewhere
            ActiveHardware = (HardwareType)(-1);
            this.console = console;
        }

        /// <summary>
        /// Creates the form and stores the ActiveHardware
        /// </summary>
        /// <param name="ActiveHardware"></param>
        public MMICommandForm(HardwareType ActiveHardware, ConsoleForm console)
        {
            InitializeComponent();

            this.ActiveHardware = ActiveHardware;

            this.console = console;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads a text file, Parses through, and stores the values
        /// </summary>
        private void helper()
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);

            String curr = "";
            List<List<String>> cmds = new List<List<string>>();
            List<String> cmd = new List<string>();
            UInt16 i = 0;
            UInt16 count = 0;

            bool gotPacket = false;
            bool first = true;

            //Reads through the File
            while ((curr = sr.ReadLine()) != null)
            {
                if (!gotPacket)
                {
                    this.numPackets = Convert.ToByte(curr,16);
                    gotPacket = true;
                }

                i++;
                cmd.Add(curr + "~");

                //add each command
                if (i == 3)
                {
                    if (first)
                    {
                        i = 0;
                        cmd = new List<string>();
                        first = false;
                    }
                    else
                    {
                        cmds.Add(cmd);
                        i = 0;
                        count++;
                        cmd = new List<string>();
                    }
                }
            }

            this.cmds = CreateCommandList(cmds);
        }

        /// <summary>
        /// Sends the Command List over the CAN Bus
        /// </summary>
        /// <param name="cmds"></param>
        /// <param name="numPackets"></param>
        private void SendCommandList(List<String> cmds, byte numPackets)
        {
            #region ValCheck
            if (cmds == null || numPackets == 0)
                return;
            #endregion

            #region setup

            TCLightMsg MsgToSend = new TCLightMsg();
            MsgToSend.ID = 0x013EE8E0 | 0x00000001;
            MsgToSend.Len = 1;
            MsgToSend.MsgType = MsgTypes.MSGTYPE_EXTENDED;
            MsgToSend.Data[0] = numPackets;

            TCLightTimestamp q = new TCLightTimestamp();
            q.millis = 1000;
            console.AddOutsideMessage(MsgToSend, q);

            PCANLight.Write(ActiveHardware, MsgToSend);


            #endregion

            #region move

            MsgToSend.ID = 0x013EE0E0 | 0x00000001;
            MsgToSend.Len = 8;
            MsgToSend.MsgType = MsgTypes.MSGTYPE_EXTENDED;
            byte count = 0;
            foreach (String cmd in cmds)
            {
                String[] cmd2 = cmd.Split('~');

                for (int i = 0; i < MsgToSend.Len; i++)
                {
                    #region Data
                    switch (i)
                    {
                        case 0:
                            ProfileParamsTb.AppendText(cmd + "\r\n");
                            MsgToSend.Data[0] = count;
                            count++;
                            break;
                        case 1:
                            MsgToSend.Data[1] = numPackets;
                            break;
                        case 2:
                            MsgToSend.Data[2] = BitConverter.GetBytes(Int16.Parse(cmd2[0]))[1];
                            break;
                        case 3:
                            MsgToSend.Data[3] = BitConverter.GetBytes(Int16.Parse(cmd2[0]))[0];
                            break;
                        case 4:
                            MsgToSend.Data[4] = BitConverter.GetBytes(UInt16.Parse(cmd2[1]))[1];
                            break;
                        case 5:
                            MsgToSend.Data[5] = BitConverter.GetBytes(UInt16.Parse(cmd2[1]))[0];
                            break;
                        case 6:
                            
                            MsgToSend.Data[6] = BitConverter.GetBytes(UInt16.Parse(cmd2[2]))[1];
                            break;
                        case 7:
                            MsgToSend.Data[7] = BitConverter.GetBytes(UInt16.Parse(cmd2[2]))[0];
                            break;
                    }
                    #endregion
                }
                console.AddOutsideMessage(MsgToSend, q);
                PCANLight.Write(ActiveHardware, MsgToSend);
            }
            #endregion

            #region Delete
            //prevents sending repeats, consider disabling if you want such a feature
            cmds = null;

            numPackets = 0;
            #endregion
        }

        /// <summary>
        /// Creates and Returns the Command List
        /// </summary>
        /// <param name="inputCmds"></param>
        /// <returns>The Command List</returns>
        private List<String> CreateCommandList(List<List<String>> inputCmds)
        {
            List<String> cmds = new List<string>();
            ProfileParamsTb.AppendText("\r\n");

            //for each command array
            foreach (List<String> cmd in inputCmds)
            {
                String cmdStr = "";

                //for each command array piece
                foreach (String cmdPiece in cmd)
                {
                    //add piece to command string
                    cmdStr += cmdPiece;
                    ProfileParamsTb.AppendText(cmdPiece + " ");
                }

                //Add command string to List
                ProfileParamsTb.AppendText("\r\n");
                cmds.Add(cmdStr);
            }

            //return list
            return cmds;
        }

        #endregion

        #region Form Functions

        /// <summary>
        /// Loads a text file, Parses through, and stores the values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadProfileBtn_Click(object sender, EventArgs e)
        {
            //if no hardware, escape
            if (ActiveHardware == (HardwareType)(-1))
                return;
            //get the file
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                helper();
                SendProfileBtn.Enabled = true;
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Sends the Commands through the CAN Bus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendProfileBtn_Click(object sender, EventArgs e)
        {
            SendCommandList(this.cmds, this.numPackets);
            SendProfileBtn.Enabled = false;
            ExecuteProfileBtn.Enabled = true;
        }

        /// <summary>
        /// Sends the Execute Command over the CAN Bus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteProfileBtn_Click(object sender, EventArgs e)
        {
            #region execute
            TCLightMsg MsgToSend = new TCLightMsg();
            MsgToSend.ID = 0x013EF0E0 | 0x00000001;
            MsgToSend.Len = 0;
            MsgToSend.MsgType = MsgTypes.MSGTYPE_EXTENDED;

            PCANLight.Write(ActiveHardware, MsgToSend);
            #endregion

            TCLightTimestamp q = new TCLightTimestamp();
            q.millis = 1000;
            console.AddOutsideMessage(MsgToSend, q);

            SendProfileBtn.Enabled = false;
            ExecuteProfileBtn.Enabled = false;
        }

        private void AlignMotorBtn_Click(object sender, EventArgs e)
        {
            TCLightMsg MsgToSend = new TCLightMsg();
            MsgToSend.ID = 0x013E20E0 | 0x00000001;
            MsgToSend.MsgType = MsgTypes.MSGTYPE_EXTENDED;
            TCLightTimestamp q = new TCLightTimestamp();
            q.millis = 1000;
            console.AddOutsideMessage(MsgToSend, q);
            PCANLight.Write(ActiveHardware, MsgToSend);
        }

        private void NPTModeBtn_Click(object sender, EventArgs e)
        {
            TCLightMsg MsgToSend = new TCLightMsg();
            MsgToSend.ID = 0x01745400 | 0x00000001 | 0x000000E0;
            MsgToSend.Len = 8;
            MsgToSend.Data[0] = (byte)0x01;
            MsgToSend.Data[1] = (byte)0x00;
            MsgToSend.Data[2] = (byte)0x00;
            MsgToSend.Data[3] = (byte)0x00;
            MsgToSend.Data[4] = (byte)0x00;
            MsgToSend.Data[5] = (byte)0x00;
            MsgToSend.Data[6] = (byte)0x00;
            MsgToSend.Data[7] = (byte)0x00;
            MsgToSend.MsgType = MsgTypes.MSGTYPE_EXTENDED;
            TCLightTimestamp q = new TCLightTimestamp();
            q.millis = 1000;
            console.AddOutsideMessage(MsgToSend, q);
            PCANLight.Write(ActiveHardware, MsgToSend);
        }

        private void PosZeroBtn_Click(object sender, EventArgs e)
        {
            TCLightMsg MsgToSend = new TCLightMsg();
            MsgToSend.ID = 0x013E2CE0 | 0x00000001;
            MsgToSend.MsgType = MsgTypes.MSGTYPE_EXTENDED;
            TCLightTimestamp q = new TCLightTimestamp();
            q.millis = 1000;
            
            console.AddOutsideMessage(MsgToSend, q);
            PCANLight.Write(ActiveHardware, MsgToSend);
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            #region setup

            TCLightMsg MsgToSend = new TCLightMsg();
            MsgToSend.ID = 0x013EE8E0 | 0x00000001;
            MsgToSend.Len = 1;
            MsgToSend.MsgType = MsgTypes.MSGTYPE_EXTENDED;
            MsgToSend.Data[0] = numPackets;

            TCLightTimestamp q = new TCLightTimestamp();
            q.millis = 1000;
            console.AddOutsideMessage(MsgToSend, q);

            PCANLight.Write(ActiveHardware, MsgToSend);


            #endregion
        }
    }
}
