using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICLRead
{
    class CanTestFuncVals
    {
        public enum gbiTestFunctions
        {
            GBI_TEST_FUNCTION_INDEX_SETUP_RAMP = 0,  // setup
            GBI_TEST_FUNCTION_INDEX_INIT_RAMP,              //   1   // INIT
            GBI_TEST_FUNCTION_INDEX_EXECUTE_RAMP,           //   2   // EXECUTE RAMP
            GBI_TEST_FUNCTION_INDEX_EXECUTE_STEP_ABS,       //   3   // EXECUTE STEP (ABS)
            GBI_TEST_FUNCTION_INDEX_EXECUTE_STEP_INCR,      //   4   // EXECUTE STEP (INCR)
            GBI_TEST_FUNCTION_INDEX_SETUP_STEP,             //   5   // SETUP STEP (INCR/ABS/MAN/AUTO)
            GBI_TEST_FUNCTION_INDEX_SET_TRIGGER,            //   6   // Set Trigger (0/1 = MAN/AUTO)
            GBI_TEST_FUNCTION_INDEX_REQ_SETTINGS,           //   7   // Request for GBI settings
            GBI_TEST_FUNCTION_INDEX_REPORT_SETTINGS,        //   8   // Report GBI settings
            GBI_TEST_FUNCTION_INDEX_SET_IDLE,               //   9   // Set DAC to the current IDLE voltage
            GBI_TEST_FUNCTION_INDEX_ADJ_IDLE,               //  10   // adjust IDLE voltage
            GBI_TEST_FUNCTION_INDEX_SET_STATIC,             //  11   // Set DAC to defined voltage
            GBI_TEST_FUNCTION_INDEX_ADJ_RAMP_START_DELAY,   //  12   // Adjust Auto_Trigger RampStart Delay
            GBI_TEST_FUNCTION_INDEX_ADJ_STEP_START_DELAY,   //  13   // Adjust Auto_Trigger StepStart Delay
            GBI_TEST_FUNCTION_INDEX_EXECUTE_STEP_AUTO,      //  14   // EXECUTE STEP (AUTO mode)
            GBI_TEST_FUNCTION_INDEX_LOAD_RAMP_DEF,          //  15   // load ramp defaults (from Util_cfg)
            GBI_TEST_FUNCTION_INDEX_COIL_DRIVE_ENABLE,      //  16   //  Enable the Coil Drive circuit
            GBI_TEST_FUNCTION_INDEX_COIL_DRIVE_DISABLE,     //  17   //  Disable the Coil Drive circuit
            GBI_TEST_FUNCTION_INDEX_REPORT_DAC_VALUE,       //  18   //  Report the active step DAC value
            GBI_TEST_FUNCTION_INDEX_ADJ_DAC_OFFSET,         //  19   //  adjust DAC offset
            gbiTestFunctionCnt
        };
    }
}
