using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hologic.CANRouter;
using Peak.Can.Light;

namespace ICLRead
{
    public partial class GBIControls : Form
    {
        #region Private Variables

        #region GBI Values

        //0
        public ushort IdleMv { get; set; }//[3:2]
        public ushort StaticMv { get; set; }//[5:4]
        public byte type { get; set; }//6
        public byte trigger { get; set; }//7    //(0/1 = MAN/AUTO) 

        //1
        public ushort startMv { get; set; }//3:2
        public ushort stopMv { get; set; }//5:4
        public ushort time { get; set; }//7:6

        //2
        public byte RampUpTime { get; set; }//2
        public byte RampDownTime { get; set; }//3
        public ushort GBIScanStartDelay { get; set; }//5:4
        public ushort GBIDetReadyDelay { get; set; }//7:6

        //3
        public ushort StepAbsMv { get; set; }//3:2
        public ushort StepIncMv { get; set; }//5:4
        public ushort StepTime { get; set; }//7:6

        //4
        public ushort DACOffsetMv { get; set; }//3:2
        //7:4 unused (probably)

        #endregion

        /// <summary>
        /// The Main Form
        /// </summary>
        private ConsoleForm console;

        /// <summary>
        /// Writes the Messages
        /// </summary>
        private MessageWriter writer;

        private MsgToResMethods M2R;

        /// <summary>
        /// Prevents Simultaneous Reading and Writing, not certain if necessary
        /// </summary>
        private System.Threading.Mutex MutExFile = new System.Threading.Mutex();

        /// <summary>
        /// The Log File Path
        /// </summary>
        private String outPath = @"C:\Users\jv0116\Desktop\Logs\GBI_Processed.txt";

        #endregion

        #region Constructor(s)

        public GBIControls(ConsoleForm console)
        {
            InitializeComponent();

            this.console = console;

            writer = new MessageWriter(this.console);

            this.M2R = console.M2R;

            //TriggerComboBox1.SelectedIndex = 0;
            //TriggerComboBox2.SelectedIndex = 0;
            //TriggerComboBox3.SelectedIndex = 0;
            //TriggerComboBox4.SelectedIndex = 0;
        }

        #endregion

        #region Helpers

        private void FileWriter()
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            //using???
            System.IO.StreamReader inFile = new System.IO.StreamReader(openFileDialog1.FileName);
            System.IO.StreamWriter outFile = new System.IO.StreamWriter( outPath, false );//overwrites

            char[] curr = new char[4];
            String outLine = "";
            uint count = 0;
            #endregion

            #region File
            outFile.WriteLine("Input Filename:  " + outPath);

            while (inFile.Peek() != -1)
            {
                inFile.Read(curr,0,4);
                outLine = "index: " + count + " Data: ";

                foreach (char a in curr)
                {
                    outLine += a;
                }

                outFile.WriteLine(outLine);
                count++;
                outLine = "";
            }

            inFile.Close();
            outFile.Close();
            #endregion
        }

        public void UpdateAll()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(delegate
                {
                    UpdateAll();
                }));
            }
            else
            {
                try
                {
                    TriggerComboBox1.SelectedIndex = trigger;
                    TriggerComboBox2.SelectedIndex = trigger;
                    TriggerComboBox3.SelectedIndex = trigger;
                    TriggerComboBox4.SelectedIndex = trigger;
                    IdleMvNtb.Value = IdleMv;
                    StaticMvNtb.Value = StaticMv;
                    RampStartNtb.Value = startMv;
                    RampEndNtb.Value = stopMv;
                    RampTimeNtb.Value = time;
                    RampUpTimeNtb.Value = RampUpTime;
                    RampDownTimeNtb.Value = RampDownTime;
                    textBox5.Text = GBIScanStartDelay.ToString();
                    StepTimeNtb.Value = StepTime;
                    textBox4.Text = GBIDetReadyDelay.ToString();
                    StepStartAbsNtb.Value = StepAbsMv;
                    StepIncrNtb.Value = StepIncMv;
                    //DAC Offset
                    //type
                }
                catch (Exception e)
                {
                    console.AddMiscMsg(e.Message);
                    console.AddMiscMsg(e.Source);
                }
            }
        }

        #endregion

        #region Form Functions

        #region Ramp

        private void SetupRampBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                (uint)PID.CAN_PID_COMMAND,
                (uint)DID.CAN_DID_THD,
                (uint)SID.CAN_SID_EXT);
            #endregion

            #region Data
            msg.Data[7] = (byte)RampDownTimeNtb.Value;
            msg.Data[6] = (byte)RampUpTimeNtb.Value;
            msg.Data[5] = (byte)RampTimeNtb.Value;
            msg.Data[4] = (byte)BitConverter.GetBytes((short)RampEndNtb.Value)[0];
            msg.Data[3] = (byte)BitConverter.GetBytes((short)RampEndNtb.Value)[1];
            msg.Data[2] = BitConverter.GetBytes((short)RampStartNtb.Value)[0];
            msg.Data[1] = BitConverter.GetBytes((short)RampStartNtb.Value)[1];
            msg.Data[0] = 0;
            #endregion

            #region Write
            writer.WriteMessage(msg);
            #endregion
        }

        private void ExecuteRampBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION ,
                          (uint)PID.CAN_PID_COMMAND ,
                          (uint)DID.CAN_DID_THD ,
                          (uint)SID.CAN_SID_AWS);;
            #endregion

            #region Data
            
            msg.Data[0] = 0x2;
            msg.Data[1] = 0;
            msg.Data[2] = 0;
            msg.Data[3] = 0;
            msg.Data[4] = 0;
            msg.Data[5] = 0;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            #region Write
            writer.WriteMessage(msg);
            #endregion
        }

        private void LoadDefaultRampBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                          (uint)PID.CAN_PID_COMMAND,
                          (uint)DID.CAN_DID_THD,
                          (uint)SID.CAN_SID_AWS);
            #endregion

            #region Data
            
            msg.Data[0] = 0xf;
            msg.Data[1] = 0;
            msg.Data[2] = 0;
            msg.Data[3] = 0;
            msg.Data[4] = 0;
            msg.Data[5] = 0;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            #region Write
            writer.WriteMessage(msg);
            #endregion
        }

        private void ReqRampSettingsBtn_Click(object sender, EventArgs e)
        {
            RequestSettings();
        }

        public void RequestSettings()
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                          (uint)PID.CAN_PID_COMMAND,
                          (uint)DID.CAN_DID_THD,
                          (uint)SID.CAN_SID_AWS);
            #endregion

            #region Data

            msg.Data[0] = 0x7;

            #endregion

            #region Write
            writer.WriteMessage(msg);
            #endregion

        }

        private void RampDelayUpBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                                (uint)PID.CAN_PID_COMMAND,
                                (uint)DID.CAN_DID_THD,
                                (uint)SID.CAN_SID_EXT);
            #endregion

            #region Data
            msg.Data[0] = (byte)CanTestFuncVals.gbiTestFunctions.GBI_TEST_FUNCTION_INDEX_ADJ_RAMP_START_DELAY;
            msg.Data[1] = 1;
            msg.Data[2] = 0;
            msg.Data[3] = 0;
            msg.Data[4] = 0;
            msg.Data[5] = 0;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            writer.WriteMessage(msg);
        }

        private void RampDelayDownBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                                (uint)PID.CAN_PID_COMMAND,
                                (uint)DID.CAN_DID_THD,
                                (uint)SID.CAN_SID_EXT);
            #endregion

            #region Data
            msg.Data[0] = (byte)CanTestFuncVals.gbiTestFunctions.GBI_TEST_FUNCTION_INDEX_ADJ_RAMP_START_DELAY;
            msg.Data[1] = 0;
            msg.Data[2] = 0;
            msg.Data[3] = 0;
            msg.Data[4] = 0;
            msg.Data[5] = 0;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            writer.WriteMessage(msg);
        }

        #endregion

        #region Step

        private void SetupStepBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION ,
                          (uint)PID.CAN_PID_COMMAND ,
                          (uint)DID.CAN_DID_THD ,
                          (uint)SID.CAN_SID_EXT);
            #endregion

            #region Data
            
            msg.Data[0] = 0x5;
            msg.Data[1] = (byte)BitConverter.GetBytes((short)StepStartAbsNtb.Value)[1];
            msg.Data[2] = (byte)BitConverter.GetBytes((short)StepStartAbsNtb.Value)[0];
            msg.Data[3] = (byte)BitConverter.GetBytes((short)StepIncrNtb.Value)[1];
            msg.Data[4] = (byte)BitConverter.GetBytes((short)StepIncrNtb.Value)[0];
            msg.Data[5] = (byte)TriggerComboBox1.SelectedIndex;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            #region Write
            writer.WriteMessage(msg);
            #endregion
        }

        private void ExecuteStepAbsBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                          (uint)PID.CAN_PID_COMMAND,
                          (uint)DID.CAN_DID_THD,
                          (uint)SID.CAN_SID_AWS);;
            #endregion

            #region Data
            
            msg.Data[0] = 0x3;
            msg.Data[1] = 0;
            msg.Data[2] = 0;
            msg.Data[3] = 0;
            msg.Data[4] = 0;
            msg.Data[5] = 0;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            #region Write
            writer.WriteMessage(msg);
            #endregion
        }

        private void ExecuteStepAutoBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                          (uint)PID.CAN_PID_COMMAND,
                          (uint)DID.CAN_DID_THD,
                          (uint)SID.CAN_SID_AWS);
            #endregion

            #region Data
            
            msg.Data[0] = 0xe;
            msg.Data[1] = 0;
            msg.Data[2] = 0;
            msg.Data[3] = 0;
            msg.Data[4] = 0;
            msg.Data[5] = 0;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            #region Write
            writer.WriteMessage(msg);
            #endregion
        }

        private void ExecuteStepIncrBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                          (uint)PID.CAN_PID_COMMAND,
                          (uint)DID.CAN_DID_THD,
                          (uint)SID.CAN_SID_AWS);
            #endregion

            #region Data
            
            msg.Data[0] = 0x4;
            msg.Data[1] = 0;
            msg.Data[2] = 0;
            msg.Data[3] = 0;
            msg.Data[4] = 0;
            msg.Data[5] = 0;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            #region Write
            writer.WriteMessage(msg);
            #endregion
        }

        private void StepDelayUpBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                                (uint)PID.CAN_PID_COMMAND,
                                (uint)DID.CAN_DID_THD,
                                (uint)SID.CAN_SID_EXT);
            #endregion

            #region Data
            msg.Data[0] = (byte)CanTestFuncVals.gbiTestFunctions.GBI_TEST_FUNCTION_INDEX_ADJ_STEP_START_DELAY;
            msg.Data[1] = 1;
            msg.Data[2] = 0;
            msg.Data[3] = 0;
            msg.Data[4] = 0;
            msg.Data[5] = 0;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            writer.WriteMessage(msg);
        }

        private void StepDelayDownBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                                (uint)PID.CAN_PID_COMMAND,
                                (uint)DID.CAN_DID_THD,
                                (uint)SID.CAN_SID_EXT);
            #endregion

            #region Data
            msg.Data[0] = (byte)CanTestFuncVals.gbiTestFunctions.GBI_TEST_FUNCTION_INDEX_ADJ_STEP_START_DELAY;
            msg.Data[1] = 0;
            msg.Data[2] = 0;
            msg.Data[3] = 0;
            msg.Data[4] = 0;
            msg.Data[5] = 0;
            msg.Data[6] = 0;
            msg.Data[7] = 0;
            #endregion

            writer.WriteMessage(msg);
        }

        #endregion

        #region Value Synchronization

        private void FileDelayNtb_ValueChanged(object sender, EventArgs e)
        {
            //RampDelayNtb.Value = FileDelayNtb.Value;
        }

        private void RampDelayNtb_ValueChanged(object sender, EventArgs e)
        {
            //FileDelayNtb.Value = RampDelayNtb.Value;
        }

        private void TriggerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION, (uint)PID.CAN_PID_COMMAND, (uint)DID.CAN_DID_THD, (uint)SID.CAN_SID_EXT);

            msg.Len = 3;

            msg.Data[0] = 6;

            if (sender == TriggerComboBox2)
            {
                msg.Data[1] = (byte)TriggerComboBox2.SelectedIndex;
            }
            else if (sender == TriggerComboBox1)
            {
                msg.Data[1] = (byte)TriggerComboBox1.SelectedIndex;
            }
            else if (sender == TriggerComboBox3)
            {
                msg.Data[1] = (byte)TriggerComboBox3.SelectedIndex;
            }
            else if (sender == TriggerComboBox4)
            {
                msg.Data[1] = (byte)TriggerComboBox4.SelectedIndex;
            }

            writer.WriteMessage(msg);
        }

        #endregion

        #region File

        private void OpenRampFileBtn_Click(object sender, EventArgs e)
        {
            //get the file
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileWriter();
                SendFileBtn.Enabled = true;
            }
        }

        private void SendFileBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region File Len

            char[] currByte = new char[2];

            System.IO.StreamReader inFile = new System.IO.StreamReader(openFileDialog1.FileName);
            UInt32 fileCharLen = (UInt32)(new System.IO.FileInfo(openFileDialog1.FileName)).Length;

            //while (inFile.Read() >= 0)//feels wrong
            //{
            //    fileCharLen++;
            //}

            Byte Len = (Byte)(Math.Ceiling((Decimal)((fileCharLen / 12) + 1)));//cast city woo woo

            inFile = new System.IO.StreamReader(openFileDialog1.FileName);

            #endregion

            #region Message Setup

            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_VAL_DBG_INFO,
                          (uint)PID.CAN_PID_COMMAND ,
                          (uint)DID.CAN_DID_THD ,
                          (uint)SID.CAN_SID_AWS);

            #endregion

            #region Data Setup
            
            String temp = "";
            Byte currPacketNum = 1;
            int count = 2;
            #endregion

            #region Data Loop

            msg.Data[1] = Len;

            while (currPacketNum <= Len )//until end
            {
                inFile.Read(currByte, 0, 2);//get a Byte

                //convert Byte-halves to a String
                temp += currByte[0];
                temp += currByte[1];

                //convert string to a byte (yes this is inefficient) then add it to the payload at 'count' position
                msg.Data[count] = Byte.Parse(temp, System.Globalization.NumberStyles.HexNumber);

                //write latest packet number to byte[0], only the last will be kept
                msg.Data[0] = currPacketNum;

                //reset the temporary string
                temp = "";

                if (count == 7)//if payload is full, write the message, reset, and increment the packet number
                {
                    writer.WriteMessage(msg);
                    count = 2;
                    currPacketNum += 1;
                }
                else//else, increment count
                {
                    count++;
                }
            }//end loop

            //write any remaining values
            if (count > 2)
            {
                for (; count < 7; count++)
                {
                    msg.Data[count] = 0;
                }
                msg.Data[0] = currPacketNum;
                //write last message
                writer.WriteMessage(msg);
            }

            #endregion

            inFile.Close();
        }

        #endregion

        #region The Other Step Thing that don asked me to do
        // how is this different???

        private void StepSendBtn_Click(object sender, EventArgs e)
        {
            #region ValCheck
            if ((int)console.GetActiveHardware() == -1)
            {
                return;
            }
            #endregion

            #region Declarations
            TCLightMsg msg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_TEST_FUNCTION,
                          (uint)PID.CAN_PID_COMMAND,
                          (uint)DID.CAN_DID_THD,
                          (uint)SID.CAN_SID_AWS);
            #endregion

            #region Data

            msg.Data[0] = 0x2;
            msg.Data[1] = 0x1;
            msg.Data[2] = (byte)BitConverter.GetBytes((short)StartValNtb.Value)[1];
            msg.Data[3] = (byte)BitConverter.GetBytes((short)StartValNtb.Value)[0];
            msg.Data[4] = (byte)BitConverter.GetBytes((short)StepValNtb.Value)[1];
            msg.Data[5] = (byte)BitConverter.GetBytes((short)StepValNtb.Value)[0];
            msg.Data[6] = (byte)BitConverter.GetBytes((short)StepCountNtb.Value)[0];
            msg.Data[7] = 0;
            #endregion

            #region Write
            writer.WriteMessage(msg);
            #endregion
        }

        #endregion

        #region Form

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            ConsoleForm.RemoveMsgToResponse((uint)MSG.CAN_VAL_NODE_TEST_FUNCTION);
            this.Hide();
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            console.ResetGBIPage();
        }

        #endregion

        #endregion
    }
}
