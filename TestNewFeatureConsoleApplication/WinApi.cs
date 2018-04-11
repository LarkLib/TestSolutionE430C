using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{

    #region SendInput
    /*
    INPUT structure
    Used by SendInput to store information for synthesizing input events such as keystrokes, mouse movement, and mouse clicks.
    Syntax
    C++
    Copy
    typedef struct tagINPUT {
      DWORD type;
      union {
        MOUSEINPUT    mi;
        KEYBDINPUT    ki;
        HARDWAREINPUT hi;
      };
    } INPUT, *PINPUT;
    Members
    type
    Type: DWORD
    The type of the input event. This member can be one of the following values. 
    Value
    Meaning
     INPUT_MOUSE0 
    The event is a mouse event. Use the mi structure of the union.

     INPUT_KEYBOARD1 
    The event is a keyboard event. Use the ki structure of the union.

     INPUT_HARDWARE2 
    The event is a hardware event. Use the hi structure of the union.
    mi
    Type: MOUSEINPUT
    The information about a simulated mouse event. 
    ki
    Type: KEYBDINPUT
    The information about a simulated keyboard event. 
    hi
    Type: HARDWAREINPUT
    The information about a simulated hardware event. 
    Remarks
     INPUT_KEYBOARD supports nonkeyboard input methods, such as handwriting recognition or voice recognition, as if it were text input by using the KEYEVENTF_UNICODE flag. For more information, see the remarks section of KEYBDINPUT.
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        /// DWORD->unsigned int
        public uint Type;
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MOUSEKEYBDHARDWAREINPUT
    {

        /// MOUSEINPUT->tagMOUSEINPUT
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;

        /// KEYBDINPUT->tagKEYBDINPUT
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;

        /// HARDWAREINPUT->tagHARDWAREINPUT
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
    }

    /*
    MOUSEINPUT structure
    Contains information about a simulated mouse event.

    Syntax

    C++
    Copy
    typedef struct tagMOUSEINPUT {
      LONG      dx;
      LONG      dy;
      DWORD     mouseData;
      DWORD     dwFlags;
      DWORD     time;
      ULONG_PTR dwExtraInfo;
    } MOUSEINPUT, *PMOUSEINPUT;

    Members
    dx
    Type: LONG

    The absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved. 
    dy
    Type: LONG

    The absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved. 
    mouseData
    Type: DWORD

    If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120.

    Windows Vista: If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated to the right; a negative value indicates that the wheel was rotated to the left. One wheel click is defined as WHEEL_DELTA, which is 120.

    If dwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be zero. 

    If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed or released. This value may be any combination of the following flags. 



    Value

    Meaning

     XBUTTON10x0001 
    Set if the first X button is pressed or released.

     XBUTTON20x0002 
    Set if the second X button is pressed or released.



    dwFlags
    Type: DWORD

    A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this member can be any reasonable combination of the following values.

    The bit flags that specify mouse button status are set to indicate changes in status, not ongoing conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, but not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released. 

    You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 



    Value

    Meaning

     MOUSEEVENTF_ABSOLUTE0x8000 
    The dx and dy members contain normalized absolute coordinates. If the flag is not set, dxand dy contain relative data (the change in position since the last reported position). This flag can be set, or not set, regardless of what kind of mouse or other pointing device, if any, is connected to the system. For further information about relative mouse motion, see the following Remarks section.

     MOUSEEVENTF_HWHEEL0x01000 
    The wheel was moved horizontally, if the mouse has a wheel. The amount of movement is specified in mouseData. 

    Windows XP/2000:  This value is not supported.

     MOUSEEVENTF_MOVE0x0001 
    Movement occurred.

     MOUSEEVENTF_MOVE_NOCOALESCE0x2000 
    The WM_MOUSEMOVE messages will not be coalesced. The default behavior is to coalesce WM_MOUSEMOVE messages. 

    Windows XP/2000:  This value is not supported.

     MOUSEEVENTF_LEFTDOWN0x0002 
    The left button was pressed.

     MOUSEEVENTF_LEFTUP0x0004 
    The left button was released.

     MOUSEEVENTF_RIGHTDOWN0x0008 
    The right button was pressed.

     MOUSEEVENTF_RIGHTUP0x0010 
    The right button was released.

     MOUSEEVENTF_MIDDLEDOWN0x0020 
    The middle button was pressed.

     MOUSEEVENTF_MIDDLEUP0x0040 
    The middle button was released.

     MOUSEEVENTF_VIRTUALDESK0x4000 
    Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.

     MOUSEEVENTF_WHEEL0x0800 
    The wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseData. 

     MOUSEEVENTF_XDOWN0x0080 
    An X button was pressed.

     MOUSEEVENTF_XUP0x0100 
    An X button was released.



    time
    Type: DWORD

    The time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp. 
    dwExtraInfo
    Type: ULONG_PTR

    An additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra information. 

    Remarks

    If the mouse has moved, indicated by MOUSEEVENTF_MOVE, dxand dy specify information about that movement. The information is specified as absolute or relative integer values. 

    If MOUSEEVENTF_ABSOLUTE value is specified, dx and dy contain normalized absolute coordinates between 0 and 65,535. The event procedure maps these coordinates onto the display surface. Coordinate (0,0) maps onto the upper-left corner of the display surface; coordinate (65535,65535) maps onto the lower-right corner. In a multimonitor system, the coordinates map to the primary monitor. 

    If MOUSEEVENTF_VIRTUALDESK is specified, the coordinates map to the entire virtual desktop.

    If the MOUSEEVENTF_ABSOLUTE value is not specified, dxand dy specify movement relative to the previous mouse event (the last reported position). Positive values mean the mouse moved right (or down); negative values mean the mouse moved left (or up). 

    Relative mouse motion is subject to the effects of the mouse speed and the two-mouse threshold values. A user sets these three values with the Pointer Speed slider of the Control Panel's Mouse Properties sheet. You can obtain and set these values using the SystemParametersInfo function. 

    The system applies two tests to the specified relative mouse movement. If the specified distance along either the x or y axis is greater than the first mouse threshold value, and the mouse speed is not zero, the system doubles the distance. If the specified distance along either the x or y axis is greater than the second mouse threshold value, and the mouse speed is equal to two, the system doubles the distance that resulted from applying the first threshold test. It is thus possible for the system to multiply specified relative mouse movement along the x or y axis by up to four times.
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {

        /// LONG->int
        public int dx;

        /// LONG->int
        public int dy;

        /// DWORD->unsigned int
        public uint mouseData;

        /// DWORD->unsigned int
        public uint dwFlags;

        /// DWORD->unsigned int
        public uint time;

        /// ULONG_PTR->unsigned int
        public uint dwExtraInfo;
    }

    /*
    KEYBDINPUT structure
    Contains information about a simulated keyboard event. 
    Syntax
    C++
    Copy
    typedef struct tagKEYBDINPUT {
      WORD      wVk;
      WORD      wScan;
      DWORD     dwFlags;
      DWORD     time;
      ULONG_PTR dwExtraInfo;
    } KEYBDINPUT, *PKEYBDINPUT;
    Members
    wVk
    Type: WORD
    A virtual-key code. The code must be a value in the range 1 to 254. If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0. 
    wScan
    Type: WORD
    A hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application. 

    dwFlags
    Type: DWORD
    Specifies various aspects of a keystroke. This member can be certain combinations of the following values. 
    Value  Meaning
    KEYEVENTF_EXTENDEDKEY0x0001 If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
    KEYEVENTF_KEYUP0x0002  If specified, the key is being released. If not specified, the key is being pressed.
    KEYEVENTF_SCANCODE0x0008 If specified, wScan identifies the key and wVk is ignored. 
    KEYEVENTF_UNICODE0x0004 If specified, the system synthesizes a VK_PACKET keystroke. 
    The wVk parameter must be zero. 
    This flag can only be combined with the KEYEVENTF_KEYUP flag. 
    For more information, see the Remarks section.

    time
    Type: DWORD
    The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp. 
    dwExtraInfo
    Type: ULONG_PTR
    An additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information. 
    Remarks
     INPUT_KEYBOARD supports nonkeyboard-input methods—such as handwriting recognition or voice recognition—as if it were text input by using the KEYEVENTF_UNICODE flag. If KEYEVENTF_UNICODE is specified, SendInput sends a WM_KEYDOWN or WM_KEYUP message to the foreground thread's message queue with wParam equal to VK_PACKET. Once GetMessage or PeekMessage obtains this message, passing the message to TranslateMessage posts a WM_CHAR message with the Unicode character originally specified by wScan. This Unicode character will automatically be converted to the appropriate ANSI value if it is posted to an ANSI window.
    Set the KEYEVENTF_SCANCODE flag to define keyboard input in terms of the scan code. This is useful to simulate a physical keystroke regardless of which keyboard is currently being used. The virtual key value of a key may alter depending on the current keyboard layout or what other keys were pressed, but the scan code will always be the same.
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {

        /// WORD->unsigned short
        public ushort wVk;

        /// WORD->unsigned short
        public ushort wScan;

        /// DWORD->unsigned int
        public uint dwFlags;

        /// DWORD->unsigned int
        public uint time;

        /// ULONG_PTR->unsigned int
        public uint dwExtraInfo;
    }

    /*
    HARDWAREINPUT structure
    Contains information about a simulated message generated by an input device other than a keyboard or mouse. 
    Syntax
    C++
    Copy
    typedef struct tagHARDWAREINPUT {
      DWORD uMsg;
      WORD  wParamL;
      WORD  wParamH;
    } HARDWAREINPUT, *PHARDWAREINPUT;
    Members
    uMsg
    Type: DWORD
    The message generated by the input hardware. 
    wParamL
    Type: WORD
    The low-order word of the lParam  parameter for uMsg. 
    wParamH
    Type: WORD
    The high-order word of the lParam  parameter for uMsg. 
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {

        /// DWORD->unsigned int
        public uint uMsg;

        /// WORD->unsigned short
        public ushort wParamL;

        /// WORD->unsigned short
        public ushort wParamH;
    }

    /*
    LASTINPUTINFO structure
    Contains the time of the last input.
    Syntax
    C++
    Copy
    typedef struct tagLASTINPUTINFO {
      UINT  cbSize;
      DWORD dwTime;
    } LASTINPUTINFO, *PLASTINPUTINFO;
    Members
    cbSize
    Type: UINT
    The size of the structure, in bytes. This member must be set to sizeof(LASTINPUTINFO). 
    dwTime
    Type: DWORD
    The tick count when the last input event was received. 
    Remarks
    This function is useful for input idle detection. For more information on tick counts, see GetTickCount.
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct LASTINPUTINFO
    {

        /// UINT->unsigned int
        public uint cbSize;

        /// DWORD->unsigned int
        public uint dwTime;
    }

    #endregion SendInput

    #region enums
    #region KeyCode enum
    public enum KeyCode : ushort
    {
        #region Media

        /// <summary>
        /// Next track if a song is playing
        /// </summary>
        MEDIA_NEXT_TRACK = 0xb0,

        /// <summary>
        /// Play pause
        /// </summary>
        MEDIA_PLAY_PAUSE = 0xb3,

        /// <summary>
        /// Previous track
        /// </summary>
        MEDIA_PREV_TRACK = 0xb1,

        /// <summary>
        /// Stop
        /// </summary>
        MEDIA_STOP = 0xb2,

        #endregion

        #region math

        /// <summary>Key "+"</summary>
        ADD = 0x6b,
        /// <summary>
        /// "*" key
        /// </summary>
        MULTIPLY = 0x6a,

        /// <summary>
        /// "/" key
        /// </summary>
        DIVIDE = 0x6f,

        /// <summary>
        /// Subtract key "-"
        /// </summary>
        SUBTRACT = 0x6d,

        #endregion

        #region Browser
        /// <summary>
        /// Go Back
        /// </summary>
        BROWSER_BACK = 0xa6,
        /// <summary>
        /// Favorites
        /// </summary>
        BROWSER_FAVORITES = 0xab,
        /// <summary>
        /// Forward
        /// </summary>
        BROWSER_FORWARD = 0xa7,
        /// <summary>
        /// Home
        /// </summary>
        BROWSER_HOME = 0xac,
        /// <summary>
        /// Refresh
        /// </summary>
        BROWSER_REFRESH = 0xa8,
        /// <summary>
        /// browser search
        /// </summary>
        BROWSER_SEARCH = 170,
        /// <summary>
        /// Stop
        /// </summary>
        BROWSER_STOP = 0xa9,
        #endregion

        #region Numpad numbers
        /// <summary>
        /// 
        /// </summary>
        NUMPAD0 = 0x60,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD1 = 0x61,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD2 = 0x62,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD3 = 0x63,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD4 = 100,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD5 = 0x65,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD6 = 0x66,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD7 = 0x67,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD8 = 0x68,
        /// <summary>
        /// 
        /// </summary>
        NUMPAD9 = 0x69,

        #endregion

        #region Fkeys
        /// <summary>
        /// F1
        /// </summary>
        F1 = 0x70,
        /// <summary>
        /// F10
        /// </summary>
        F10 = 0x79,
        /// <summary>
        /// 
        /// </summary>
        F11 = 0x7a,
        /// <summary>
        /// 
        /// </summary>
        F12 = 0x7b,
        /// <summary>
        /// 
        /// </summary>
        F13 = 0x7c,
        /// <summary>
        /// 
        /// </summary>
        F14 = 0x7d,
        /// <summary>
        /// 
        /// </summary>
        F15 = 0x7e,
        /// <summary>
        /// 
        /// </summary>
        F16 = 0x7f,
        /// <summary>
        /// 
        /// </summary>
        F17 = 0x80,
        /// <summary>
        /// 
        /// </summary>
        F18 = 0x81,
        /// <summary>
        /// 
        /// </summary>
        F19 = 130,
        /// <summary>
        /// 
        /// </summary>
        F2 = 0x71,
        /// <summary>
        /// 
        /// </summary>
        F20 = 0x83,
        /// <summary>
        /// 
        /// </summary>
        F21 = 0x84,
        /// <summary>
        /// 
        /// </summary>
        F22 = 0x85,
        /// <summary>
        /// 
        /// </summary>
        F23 = 0x86,
        /// <summary>
        /// 
        /// </summary>
        F24 = 0x87,
        /// <summary>
        /// 
        /// </summary>
        F3 = 0x72,
        /// <summary>
        /// 
        /// </summary>
        F4 = 0x73,
        /// <summary>
        /// 
        /// </summary>
        F5 = 0x74,
        /// <summary>
        /// 
        /// </summary>
        F6 = 0x75,
        /// <summary>
        /// 
        /// </summary>
        F7 = 0x76,
        /// <summary>
        /// 
        /// </summary>
        F8 = 0x77,
        /// <summary>
        /// 
        /// </summary>
        F9 = 120,

        #endregion

        #region Other
        /// <summary>
        /// 
        /// </summary>
        OEM_1 = 0xba,
        /// <summary>
        /// 
        /// </summary>
        OEM_102 = 0xe2,
        /// <summary>
        /// 
        /// </summary>
        OEM_2 = 0xbf,
        /// <summary>
        /// 
        /// </summary>
        OEM_3 = 0xc0,
        /// <summary>
        /// 
        /// </summary>
        OEM_4 = 0xdb,
        /// <summary>
        /// 
        /// </summary>
        OEM_5 = 220,
        /// <summary>
        /// 
        /// </summary>
        OEM_6 = 0xdd,
        /// <summary>
        /// 
        /// </summary>
        OEM_7 = 0xde,
        /// <summary>
        /// 
        /// </summary>
        OEM_8 = 0xdf,
        /// <summary>
        /// 
        /// </summary>
        OEM_CLEAR = 0xfe,
        /// <summary>
        /// 
        /// </summary>
        OEM_COMMA = 0xbc,
        /// <summary>
        /// 
        /// </summary>
        OEM_MINUS = 0xbd,
        /// <summary>
        /// 
        /// </summary>
        OEM_PERIOD = 190,
        /// <summary>
        /// 
        /// </summary>
        OEM_PLUS = 0xbb,

        #endregion

        #region KEYS

        /// <summary>
        /// 
        /// </summary>
        KEY_0 = 0x30,
        /// <summary>
        /// 
        /// </summary>
        KEY_1 = 0x31,
        /// <summary>
        /// 
        /// </summary>
        KEY_2 = 50,
        /// <summary>
        /// 
        /// </summary>
        KEY_3 = 0x33,
        /// <summary>
        /// 
        /// </summary>
        KEY_4 = 0x34,
        /// <summary>
        /// 
        /// </summary>
        KEY_5 = 0x35,
        /// <summary>
        /// 
        /// </summary>
        KEY_6 = 0x36,
        /// <summary>
        /// 
        /// </summary>
        KEY_7 = 0x37,
        /// <summary>
        /// 
        /// </summary>
        KEY_8 = 0x38,
        /// <summary>
        /// 
        /// </summary>
        KEY_9 = 0x39,
        /// <summary>
        /// 
        /// </summary>
        KEY_A = 0x41,
        /// <summary>
        /// 
        /// </summary>
        KEY_B = 0x42,
        /// <summary>
        /// 
        /// </summary>
        KEY_C = 0x43,
        /// <summary>
        /// 
        /// </summary>
        KEY_D = 0x44,
        /// <summary>
        /// 
        /// </summary>
        KEY_E = 0x45,
        /// <summary>
        /// 
        /// </summary>
        KEY_F = 70,
        /// <summary>
        /// 
        /// </summary>
        KEY_G = 0x47,
        /// <summary>
        /// 
        /// </summary>
        KEY_H = 0x48,
        /// <summary>
        /// 
        /// </summary>
        KEY_I = 0x49,
        /// <summary>
        /// 
        /// </summary>
        KEY_J = 0x4a,
        /// <summary>
        /// 
        /// </summary>
        KEY_K = 0x4b,
        /// <summary>
        /// 
        /// </summary>
        KEY_L = 0x4c,
        /// <summary>
        /// 
        /// </summary>
        KEY_M = 0x4d,
        /// <summary>
        /// 
        /// </summary>
        KEY_N = 0x4e,
        /// <summary>
        /// 
        /// </summary>
        KEY_O = 0x4f,
        /// <summary>
        /// 
        /// </summary>
        KEY_P = 80,
        /// <summary>
        /// 
        /// </summary>
        KEY_Q = 0x51,
        /// <summary>
        /// 
        /// </summary>
        KEY_R = 0x52,
        /// <summary>
        /// 
        /// </summary>
        KEY_S = 0x53,
        /// <summary>
        /// 
        /// </summary>
        KEY_T = 0x54,
        /// <summary>
        /// 
        /// </summary>
        KEY_U = 0x55,
        /// <summary>
        /// 
        /// </summary>
        KEY_V = 0x56,
        /// <summary>
        /// 
        /// </summary>
        KEY_W = 0x57,
        /// <summary>
        /// 
        /// </summary>
        KEY_X = 0x58,
        /// <summary>
        /// 
        /// </summary>
        KEY_Y = 0x59,
        /// <summary>
        /// 
        /// </summary>
        KEY_Z = 90,

        #endregion

        #region volume
        /// <summary>
        /// Decrese volume
        /// </summary>
        VOLUME_DOWN = 0xae,

        /// <summary>
        /// Mute volume
        /// </summary>
        VOLUME_MUTE = 0xad,

        /// <summary>
        /// Increase volue
        /// </summary>
        VOLUME_UP = 0xaf,

        #endregion


        /// <summary>
        /// Take snapshot of the screen and place it on the clipboard
        /// </summary>
        SNAPSHOT = 0x2c,

        /// <summary>Send right click from keyboard "key that is 2 keys to the right of space bar"</summary>
        RightClick = 0x5d,

        /// <summary>
        /// Go Back or delete
        /// </summary>
        BACKSPACE = 8,

        /// <summary>
        /// Control + Break "When debuging if you step into an infinite loop this will stop debug"
        /// </summary>
        CANCEL = 3,
        /// <summary>
        /// Caps lock key to send cappital letters
        /// </summary>
        CAPS_LOCK = 20,
        /// <summary>
        /// Ctlr key
        /// </summary>
        CONTROL = 0x11,

        /// <summary>
        /// Alt key
        /// </summary>
        ALT = 18,

        /// <summary>
        /// "." key
        /// </summary>
        DECIMAL = 110,

        /// <summary>
        /// Delete Key
        /// </summary>
        DELETE = 0x2e,


        /// <summary>
        /// Arrow down key
        /// </summary>
        DOWN = 40,

        /// <summary>
        /// End key
        /// </summary>
        END = 0x23,

        /// <summary>
        /// Escape key
        /// </summary>
        ESC = 0x1b,

        /// <summary>
        /// Home key
        /// </summary>
        HOME = 0x24,

        /// <summary>
        /// Insert key
        /// </summary>
        INSERT = 0x2d,

        /// <summary>
        /// Open my computer
        /// </summary>
        LAUNCH_APP1 = 0xb6,
        /// <summary>
        /// Open calculator
        /// </summary>
        LAUNCH_APP2 = 0xb7,

        /// <summary>
        /// Open default email in my case outlook
        /// </summary>
        LAUNCH_MAIL = 180,

        /// <summary>
        /// Opend default media player (itunes, winmediaplayer, etc)
        /// </summary>
        LAUNCH_MEDIA_SELECT = 0xb5,

        /// <summary>
        /// Left control
        /// </summary>
        LCONTROL = 0xa2,

        /// <summary>
        /// Left arrow
        /// </summary>
        LEFT = 0x25,

        /// <summary>
        /// Left shift
        /// </summary>
        LSHIFT = 160,

        /// <summary>
        /// left windows key
        /// </summary>
        LWIN = 0x5b,


        /// <summary>
        /// Next "page down"
        /// </summary>
        PAGEDOWN = 0x22,

        /// <summary>
        /// Num lock to enable typing numbers
        /// </summary>
        NUMLOCK = 0x90,

        /// <summary>
        /// Page up key
        /// </summary>
        PAGE_UP = 0x21,

        /// <summary>
        /// Right control
        /// </summary>
        RCONTROL = 0xa3,

        /// <summary>
        /// Return key
        /// </summary>
        ENTER = 13,

        /// <summary>
        /// Right arrow key
        /// </summary>
        RIGHT = 0x27,

        /// <summary>
        /// Right shift
        /// </summary>
        RSHIFT = 0xa1,

        /// <summary>
        /// Right windows key
        /// </summary>
        RWIN = 0x5c,

        /// <summary>
        /// Shift key
        /// </summary>
        SHIFT = 0x10,

        /// <summary>
        /// Space back key
        /// </summary>
        SPACE_BAR = 0x20,

        /// <summary>
        /// Tab key
        /// </summary>
        TAB = 9,

        /// <summary>
        /// Up arrow key
        /// </summary>
        UP = 0x26,

    }

    [Flags]
    public enum Keys
    {
        //
        // Summary:
        //     The bitmask to extract modifiers from a key value.
        Modifiers = -65536,
        //
        // Summary:
        //     No key pressed.
        None = 0,
        //
        // Summary:
        //     The left mouse button.
        LButton = 1,
        //
        // Summary:
        //     The right mouse button.
        RButton = 2,
        //
        // Summary:
        //     The CANCEL key.
        Cancel = 3,
        //
        // Summary:
        //     The middle mouse button (three-button mouse).
        MButton = 4,
        //
        // Summary:
        //     The first x mouse button (five-button mouse).
        XButton1 = 5,
        //
        // Summary:
        //     The second x mouse button (five-button mouse).
        XButton2 = 6,
        //
        // Summary:
        //     The BACKSPACE key.
        Back = 8,
        //
        // Summary:
        //     The TAB key.
        Tab = 9,
        //
        // Summary:
        //     The LINEFEED key.
        LineFeed = 10,
        //
        // Summary:
        //     The CLEAR key.
        Clear = 12,
        //
        // Summary:
        //     The RETURN key.
        Return = 13,
        //
        // Summary:
        //     The ENTER key.
        Enter = 13,
        //
        // Summary:
        //     The SHIFT key.
        ShiftKey = 16,
        //
        // Summary:
        //     The CTRL key.
        ControlKey = 17,
        //
        // Summary:
        //     The ALT key.
        Menu = 18,
        //
        // Summary:
        //     The PAUSE key.
        Pause = 19,
        //
        // Summary:
        //     The CAPS LOCK key.
        Capital = 20,
        //
        // Summary:
        //     The CAPS LOCK key.
        CapsLock = 20,
        //
        // Summary:
        //     The IME Kana mode key.
        KanaMode = 21,
        //
        // Summary:
        //     The IME Hanguel mode key. (maintained for compatibility; use HangulMode)
        HanguelMode = 21,
        //
        // Summary:
        //     The IME Hangul mode key.
        HangulMode = 21,
        //
        // Summary:
        //     The IME Junja mode key.
        JunjaMode = 23,
        //
        // Summary:
        //     The IME final mode key.
        FinalMode = 24,
        //
        // Summary:
        //     The IME Hanja mode key.
        HanjaMode = 25,
        //
        // Summary:
        //     The IME Kanji mode key.
        KanjiMode = 25,
        //
        // Summary:
        //     The ESC key.
        Escape = 27,
        //
        // Summary:
        //     The IME convert key.
        IMEConvert = 28,
        //
        // Summary:
        //     The IME nonconvert key.
        IMENonconvert = 29,
        //
        // Summary:
        //     The IME accept key, replaces System.Windows.Forms.Keys.IMEAceept.
        IMEAccept = 30,
        //
        // Summary:
        //     The IME accept key. Obsolete, use System.Windows.Forms.Keys.IMEAccept instead.
        IMEAceept = 30,
        //
        // Summary:
        //     The IME mode change key.
        IMEModeChange = 31,
        //
        // Summary:
        //     The SPACEBAR key.
        Space = 32,
        //
        // Summary:
        //     The PAGE UP key.
        Prior = 33,
        //
        // Summary:
        //     The PAGE UP key.
        PageUp = 33,
        //
        // Summary:
        //     The PAGE DOWN key.
        Next = 34,
        //
        // Summary:
        //     The PAGE DOWN key.
        PageDown = 34,
        //
        // Summary:
        //     The END key.
        End = 35,
        //
        // Summary:
        //     The HOME key.
        Home = 36,
        //
        // Summary:
        //     The LEFT ARROW key.
        Left = 37,
        //
        // Summary:
        //     The UP ARROW key.
        Up = 38,
        //
        // Summary:
        //     The RIGHT ARROW key.
        Right = 39,
        //
        // Summary:
        //     The DOWN ARROW key.
        Down = 40,
        //
        // Summary:
        //     The SELECT key.
        Select = 41,
        //
        // Summary:
        //     The PRINT key.
        Print = 42,
        //
        // Summary:
        //     The EXECUTE key.
        Execute = 43,
        //
        // Summary:
        //     The PRINT SCREEN key.
        Snapshot = 44,
        //
        // Summary:
        //     The PRINT SCREEN key.
        PrintScreen = 44,
        //
        // Summary:
        //     The INS key.
        Insert = 45,
        //
        // Summary:
        //     The DEL key.
        Delete = 46,
        //
        // Summary:
        //     The HELP key.
        Help = 47,
        //
        // Summary:
        //     The 0 key.
        D0 = 48,
        //
        // Summary:
        //     The 1 key.
        D1 = 49,
        //
        // Summary:
        //     The 2 key.
        D2 = 50,
        //
        // Summary:
        //     The 3 key.
        D3 = 51,
        //
        // Summary:
        //     The 4 key.
        D4 = 52,
        //
        // Summary:
        //     The 5 key.
        D5 = 53,
        //
        // Summary:
        //     The 6 key.
        D6 = 54,
        //
        // Summary:
        //     The 7 key.
        D7 = 55,
        //
        // Summary:
        //     The 8 key.
        D8 = 56,
        //
        // Summary:
        //     The 9 key.
        D9 = 57,
        //
        // Summary:
        //     The A key.
        A = 65,
        //
        // Summary:
        //     The B key.
        B = 66,
        //
        // Summary:
        //     The C key.
        C = 67,
        //
        // Summary:
        //     The D key.
        D = 68,
        //
        // Summary:
        //     The E key.
        E = 69,
        //
        // Summary:
        //     The F key.
        F = 70,
        //
        // Summary:
        //     The G key.
        G = 71,
        //
        // Summary:
        //     The H key.
        H = 72,
        //
        // Summary:
        //     The I key.
        I = 73,
        //
        // Summary:
        //     The J key.
        J = 74,
        //
        // Summary:
        //     The K key.
        K = 75,
        //
        // Summary:
        //     The L key.
        L = 76,
        //
        // Summary:
        //     The M key.
        M = 77,
        //
        // Summary:
        //     The N key.
        N = 78,
        //
        // Summary:
        //     The O key.
        O = 79,
        //
        // Summary:
        //     The P key.
        P = 80,
        //
        // Summary:
        //     The Q key.
        Q = 81,
        //
        // Summary:
        //     The R key.
        R = 82,
        //
        // Summary:
        //     The S key.
        S = 83,
        //
        // Summary:
        //     The T key.
        T = 84,
        //
        // Summary:
        //     The U key.
        U = 85,
        //
        // Summary:
        //     The V key.
        V = 86,
        //
        // Summary:
        //     The W key.
        W = 87,
        //
        // Summary:
        //     The X key.
        X = 88,
        //
        // Summary:
        //     The Y key.
        Y = 89,
        //
        // Summary:
        //     The Z key.
        Z = 90,
        //
        // Summary:
        //     The left Windows logo key (Microsoft Natural Keyboard).
        LWin = 91,
        //
        // Summary:
        //     The right Windows logo key (Microsoft Natural Keyboard).
        RWin = 92,
        //
        // Summary:
        //     The application key (Microsoft Natural Keyboard).
        Apps = 93,
        //
        // Summary:
        //     The computer sleep key.
        Sleep = 95,
        //
        // Summary:
        //     The 0 key on the numeric keypad.
        NumPad0 = 96,
        //
        // Summary:
        //     The 1 key on the numeric keypad.
        NumPad1 = 97,
        //
        // Summary:
        //     The 2 key on the numeric keypad.
        NumPad2 = 98,
        //
        // Summary:
        //     The 3 key on the numeric keypad.
        NumPad3 = 99,
        //
        // Summary:
        //     The 4 key on the numeric keypad.
        NumPad4 = 100,
        //
        // Summary:
        //     The 5 key on the numeric keypad.
        NumPad5 = 101,
        //
        // Summary:
        //     The 6 key on the numeric keypad.
        NumPad6 = 102,
        //
        // Summary:
        //     The 7 key on the numeric keypad.
        NumPad7 = 103,
        //
        // Summary:
        //     The 8 key on the numeric keypad.
        NumPad8 = 104,
        //
        // Summary:
        //     The 9 key on the numeric keypad.
        NumPad9 = 105,
        //
        // Summary:
        //     The multiply key.
        Multiply = 106,
        //
        // Summary:
        //     The add key.
        Add = 107,
        //
        // Summary:
        //     The separator key.
        Separator = 108,
        //
        // Summary:
        //     The subtract key.
        Subtract = 109,
        //
        // Summary:
        //     The decimal key.
        Decimal = 110,
        //
        // Summary:
        //     The divide key.
        Divide = 111,
        //
        // Summary:
        //     The F1 key.
        F1 = 112,
        //
        // Summary:
        //     The F2 key.
        F2 = 113,
        //
        // Summary:
        //     The F3 key.
        F3 = 114,
        //
        // Summary:
        //     The F4 key.
        F4 = 115,
        //
        // Summary:
        //     The F5 key.
        F5 = 116,
        //
        // Summary:
        //     The F6 key.
        F6 = 117,
        //
        // Summary:
        //     The F7 key.
        F7 = 118,
        //
        // Summary:
        //     The F8 key.
        F8 = 119,
        //
        // Summary:
        //     The F9 key.
        F9 = 120,
        //
        // Summary:
        //     The F10 key.
        F10 = 121,
        //
        // Summary:
        //     The F11 key.
        F11 = 122,
        //
        // Summary:
        //     The F12 key.
        F12 = 123,
        //
        // Summary:
        //     The F13 key.
        F13 = 124,
        //
        // Summary:
        //     The F14 key.
        F14 = 125,
        //
        // Summary:
        //     The F15 key.
        F15 = 126,
        //
        // Summary:
        //     The F16 key.
        F16 = 127,
        //
        // Summary:
        //     The F17 key.
        F17 = 128,
        //
        // Summary:
        //     The F18 key.
        F18 = 129,
        //
        // Summary:
        //     The F19 key.
        F19 = 130,
        //
        // Summary:
        //     The F20 key.
        F20 = 131,
        //
        // Summary:
        //     The F21 key.
        F21 = 132,
        //
        // Summary:
        //     The F22 key.
        F22 = 133,
        //
        // Summary:
        //     The F23 key.
        F23 = 134,
        //
        // Summary:
        //     The F24 key.
        F24 = 135,
        //
        // Summary:
        //     The NUM LOCK key.
        NumLock = 144,
        //
        // Summary:
        //     The SCROLL LOCK key.
        Scroll = 145,
        //
        // Summary:
        //     The left SHIFT key.
        LShiftKey = 160,
        //
        // Summary:
        //     The right SHIFT key.
        RShiftKey = 161,
        //
        // Summary:
        //     The left CTRL key.
        LControlKey = 162,
        //
        // Summary:
        //     The right CTRL key.
        RControlKey = 163,
        //
        // Summary:
        //     The left ALT key.
        LMenu = 164,
        //
        // Summary:
        //     The right ALT key.
        RMenu = 165,
        //
        // Summary:
        //     The browser back key (Windows 2000 or later).
        BrowserBack = 166,
        //
        // Summary:
        //     The browser forward key (Windows 2000 or later).
        BrowserForward = 167,
        //
        // Summary:
        //     The browser refresh key (Windows 2000 or later).
        BrowserRefresh = 168,
        //
        // Summary:
        //     The browser stop key (Windows 2000 or later).
        BrowserStop = 169,
        //
        // Summary:
        //     The browser search key (Windows 2000 or later).
        BrowserSearch = 170,
        //
        // Summary:
        //     The browser favorites key (Windows 2000 or later).
        BrowserFavorites = 171,
        //
        // Summary:
        //     The browser home key (Windows 2000 or later).
        BrowserHome = 172,
        //
        // Summary:
        //     The volume mute key (Windows 2000 or later).
        VolumeMute = 173,
        //
        // Summary:
        //     The volume down key (Windows 2000 or later).
        VolumeDown = 174,
        //
        // Summary:
        //     The volume up key (Windows 2000 or later).
        VolumeUp = 175,
        //
        // Summary:
        //     The media next track key (Windows 2000 or later).
        MediaNextTrack = 176,
        //
        // Summary:
        //     The media previous track key (Windows 2000 or later).
        MediaPreviousTrack = 177,
        //
        // Summary:
        //     The media Stop key (Windows 2000 or later).
        MediaStop = 178,
        //
        // Summary:
        //     The media play pause key (Windows 2000 or later).
        MediaPlayPause = 179,
        //
        // Summary:
        //     The launch mail key (Windows 2000 or later).
        LaunchMail = 180,
        //
        // Summary:
        //     The select media key (Windows 2000 or later).
        SelectMedia = 181,
        //
        // Summary:
        //     The start application one key (Windows 2000 or later).
        LaunchApplication1 = 182,
        //
        // Summary:
        //     The start application two key (Windows 2000 or later).
        LaunchApplication2 = 183,
        //
        // Summary:
        //     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
        OemSemicolon = 186,
        //
        // Summary:
        //     The OEM 1 key.
        Oem1 = 186,
        //
        // Summary:
        //     The OEM plus key on any country/region keyboard (Windows 2000 or later).
        Oemplus = 187,
        //
        // Summary:
        //     The OEM comma key on any country/region keyboard (Windows 2000 or later).
        Oemcomma = 188,
        //
        // Summary:
        //     The OEM minus key on any country/region keyboard (Windows 2000 or later).
        OemMinus = 189,
        //
        // Summary:
        //     The OEM period key on any country/region keyboard (Windows 2000 or later).
        OemPeriod = 190,
        //
        // Summary:
        //     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
        OemQuestion = 191,
        //
        // Summary:
        //     The OEM 2 key.
        Oem2 = 191,
        //
        // Summary:
        //     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
        Oemtilde = 192,
        //
        // Summary:
        //     The OEM 3 key.
        Oem3 = 192,
        //
        // Summary:
        //     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
        OemOpenBrackets = 219,
        //
        // Summary:
        //     The OEM 4 key.
        Oem4 = 219,
        //
        // Summary:
        //     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
        OemPipe = 220,
        //
        // Summary:
        //     The OEM 5 key.
        Oem5 = 220,
        //
        // Summary:
        //     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
        OemCloseBrackets = 221,
        //
        // Summary:
        //     The OEM 6 key.
        Oem6 = 221,
        //
        // Summary:
        //     The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
        OemQuotes = 222,
        //
        // Summary:
        //     The OEM 7 key.
        Oem7 = 222,
        //
        // Summary:
        //     The OEM 8 key.
        Oem8 = 223,
        //
        // Summary:
        //     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000
        //     or later).
        OemBackslash = 226,
        //
        // Summary:
        //     The OEM 102 key.
        Oem102 = 226,
        //
        // Summary:
        //     The PROCESS KEY key.
        ProcessKey = 229,
        //
        // Summary:
        //     Used to pass Unicode characters as if they were keystrokes. The Packet key value
        //     is the low word of a 32-bit virtual-key value used for non-keyboard input methods.
        Packet = 231,
        //
        // Summary:
        //     The ATTN key.
        Attn = 246,
        //
        // Summary:
        //     The CRSEL key.
        Crsel = 247,
        //
        // Summary:
        //     The EXSEL key.
        Exsel = 248,
        //
        // Summary:
        //     The ERASE EOF key.
        EraseEof = 249,
        //
        // Summary:
        //     The PLAY key.
        Play = 250,
        //
        // Summary:
        //     The ZOOM key.
        Zoom = 251,
        //
        // Summary:
        //     A constant reserved for future use.
        NoName = 252,
        //
        // Summary:
        //     The PA1 key.
        Pa1 = 253,
        //
        // Summary:
        //     The CLEAR key.
        OemClear = 254,
        //
        // Summary:
        //     The bitmask to extract a key code from a key value.
        KeyCode = 65535,
        //
        // Summary:
        //     The SHIFT modifier key.
        Shift = 65536,
        //
        // Summary:
        //     The CTRL modifier key.
        Control = 131072,
        //
        // Summary:
        //     The ALT modifier key.
        Alt = 262144
    }
    #endregion KeyCode enum

    [Flags]
    enum MouseEventFlag : uint
    {
        Move = 0x0001,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        XDown = 0x0080,
        XUp = 0x0100,
        Wheel = 0x0800,
        VirtualDesk = 0x4000,
        Absolute = 0x8000
    }
    #endregion enums

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {

        /// LONG->int
        public int x;

        /// LONG->int
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [ComVisible(true)]
    class WinApi
    {
        /*
        Windows Message List
        #define WM_NULL                          0x0000
        #define WM_CREATE                        0x0001
        #define WM_DESTROY                       0x0002
        #define WM_MOVE                          0x0003
        #define WM_SIZE                          0x0005 

        #define WM_ACTIVATE                      0x0006
         /*
         * WM_ACTIVATE state values
         * /
        #define WA_INACTIVE      0
        #define WA_ACTIVE        1
        #define WA_CLICKACTIVE   2
        #define WM_SETFOCUS                      0x0007
        #define WM_KILLFOCUS                     0x0008
        #define WM_ENABLE                        0x000A
        #define WM_SETREDRAW                     0x000B
        #define WM_SETTEXT                       0x000C
        #define WM_GETTEXT                       0x000D
        #define WM_GETTEXTLENGTH                 0x000E
        #define WM_PAINT                         0x000F
        #define WM_CLOSE                         0x0010
        #define WM_QUERYENDSESSION               0x0011
        #define WM_QUIT                          0x0012
        #define WM_QUERYOPEN                     0x0013
        #define WM_ERASEBKGND                    0x0014
        #define WM_SYSCOLORCHANGE                0x0015
        #define WM_ENDSESSION                    0x0016
        #define WM_SHOWWINDOW                    0x0018
        #define WM_WININICHANGE                  0x001A
        #if (WINVER >= 0x0400)
        #define WM_SETTINGCHANGE                 WM_WININICHANGE
        #endif /* WINVER >= 0x0400 * /
        #define WM_DEVMODECHANGE                 0x001B
        #define WM_ACTIVATEAPP                   0x001C
        #define WM_FONTCHANGE                    0x001D
        #define WM_TIMECHANGE                    0x001E
        #define WM_CANCELMODE                    0x001F
        #define WM_SETCURSOR                     0x0020
        #define WM_MOUSEACTIVATE                 0x0021
        #define WM_CHILDACTIVATE                 0x0022
        #define WM_QUEUESYNC                     0x0023
        #define WM_GETMINMAXINFO                 0x0024
                // end_r_winuser
                /*
                * Struct pointed to by WM_GETMINMAXINFO lParam
                * /
                typedef struct tagMINMAXINFO
                {
                    POINT ptReserved;
                    POINT ptMaxSize;
                    POINT ptMaxPosition;
                    POINT ptMinTrackSize;
                    POINT ptMaxTrackSize;
                }
                MINMAXINFO, *PMINMAXINFO, *LPMINMAXINFO;
        // begin_r_winuser
        #define WM_PAINTICON                     0x0026
        #define WM_ICONERASEBKGND                0x0027
        #define WM_NEXTDLGCTL                    0x0028
        #define WM_SPOOLERSTATUS                 0x002A
        #define WM_DRAWITEM                      0x002B
        #define WM_MEASUREITEM                   0x002C
        #define WM_DELETEITEM                    0x002D
        #define WM_VKEYTOITEM                    0x002E
        #define WM_CHARTOITEM                    0x002F
        #define WM_SETFONT                       0x0030
        #define WM_GETFONT                       0x0031
        #define WM_SETHOTKEY                     0x0032
        #define WM_GETHOTKEY                     0x0033
        #define WM_QUERYDRAGICON                 0x0037
        #define WM_COMPAREITEM                   0x0039
        #if(WINVER >= 0x0500)
        #define WM_GETOBJECT                     0x003D
        #endif /* WINVER >= 0x0500 * /
        #define WM_COMPACTING                    0x0041
        #define WM_COMMNOTIFY                    0x0044   /* no longer suported * /
        #define WM_WINDOWPOSCHANGING             0x0046
        #define WM_WINDOWPOSCHANGED              0x0047
        #define WM_POWER                         0x0048
                /*
                * wParam for WM_POWER window message and DRV_POWER driver notification
                * /
        #define PWR_OK               1
        #define PWR_FAIL             (-1)
        #define PWR_SUSPENDREQUEST   1
        #define PWR_SUSPENDRESUME    2
        #define PWR_CRITICALRESUME   3
        #define WM_COPYDATA                      0x004A
        #define WM_CANCELJOURNAL                 0x004B
        // end_r_winuser
        /*
         * lParam of WM_COPYDATA message points to...
        * /
         typedef struct tagCOPYDATASTRUCT
                {
                    DWORD dwData;
                    DWORD cbData;
                    PVOID lpData;
                }
                COPYDATASTRUCT, *PCOPYDATASTRUCT;
        // begin_r_winuser
        #if(WINVER >= 0x0400)
        #define WM_NOTIFY                        0x004E
        #define WM_INPUTLANGCHANGEREQUEST        0x0050
        #define WM_INPUTLANGCHANGE               0x0051
        #define WM_TCARD                         0x0052
        #define WM_HELP                          0x0053
        #define WM_USERCHANGED                   0x0054
        #define WM_NOTIFYFORMAT                  0x0055
        #define NFR_ANSI                              1
        #define NFR_UNICODE                           2
        #define NF_QUERY                              3
        #define NF_REQUERY                            4
        #define WM_CONTEXTMENU                   0x007B
        #define WM_STYLECHANGING                 0x007C
        #define WM_STYLECHANGED                  0x007D
        #define WM_DISPLAYCHANGE                 0x007E
        #define WM_GETICON                       0x007F
        #define WM_SETICON                       0x0080
        #endif /* WINVER >= 0x0400 * /
        #define WM_NCCREATE                      0x0081
        #define WM_NCDESTROY                     0x0082
        #define WM_NCCALCSIZE                    0x0083
        #define WM_NCHITTEST                     0x0084
        #define WM_NCPAINT                       0x0085
        #define WM_NCACTIVATE                    0x0086
        #define WM_GETDLGCODE                    0x0087
        #define WM_SYNCPAINT                     0x0088
        #define WM_NCMOUSEMOVE                   0x00A0
        #define WM_NCLBUTTONDOWN                 0x00A1
        #define WM_NCLBUTTONUP                   0x00A2
        #define WM_NCLBUTTONDBLCLK               0x00A3
        #define WM_NCRBUTTONDOWN                 0x00A4
        #define WM_NCRBUTTONUP                   0x00A5
        #define WM_NCRBUTTONDBLCLK               0x00A6
        #define WM_NCMBUTTONDOWN                 0x00A7
        #define WM_NCMBUTTONUP                   0x00A8
        #define WM_NCMBUTTONDBLCLK               0x00A9
        #define WM_KEYFIRST                      0x0100
        #define WM_KEYDOWN                       0x0100
        #define WM_KEYUP                         0x0101
        #define WM_CHAR                          0x0102
        #define WM_DEADCHAR                      0x0103
        #define WM_SYSKEYDOWN                    0x0104
        #define WM_SYSKEYUP                      0x0105
        #define WM_SYSCHAR                       0x0106
        #define WM_SYSDEADCHAR                   0x0107
        #define WM_KEYLAST                       0x0108
        #if(WINVER >= 0x0400)
        #define WM_IME_STARTCOMPOSITION          0x010D
        #define WM_IME_ENDCOMPOSITION            0x010E
        #define WM_IME_COMPOSITION               0x010F
        #define WM_IME_KEYLAST                   0x010F
        #endif /* WINVER >= 0x0400 * /
        #define WM_INITDIALOG                    0x0110
        #define WM_COMMAND                       0x0111
        #define WM_SYSCOMMAND                    0x0112
        #define WM_TIMER                         0x0113
        #define WM_HSCROLL                       0x0114
        #define WM_VSCROLL                       0x0115
        #define WM_INITMENU                      0x0116
        #define WM_INITMENUPOPUP                 0x0117
        #define WM_MENUSELECT                    0x011F
        #define WM_MENUCHAR                      0x0120
        #define WM_ENTERIDLE                     0x0121
        #if(WINVER >= 0x0500)
        #define WM_MENURBUTTONUP                 0x0122
        #define WM_MENUDRAG                      0x0123
        #define WM_MENUGETOBJECT                 0x0124
        #define WM_UNINITMENUPOPUP               0x0125
        #define WM_MENUCOMMAND                   0x0126
        #endif /* WINVER >= 0x0500 * / 

        #define WM_CTLCOLORMSGBOX                0x0132
        #define WM_CTLCOLOREDIT                  0x0133
        #define WM_CTLCOLORLISTBOX               0x0134
        #define WM_CTLCOLORBTN                   0x0135
        #define WM_CTLCOLORDLG                   0x0136
        #define WM_CTLCOLORSCROLLBAR             0x0137
        #define WM_CTLCOLORSTATIC                0x0138
        #define WM_MOUSEFIRST                    0x0200
        #define WM_MOUSEMOVE                     0x0200
        #define WM_LBUTTONDOWN                   0x0201
        #define WM_LBUTTONUP                     0x0202
        #define WM_LBUTTONDBLCLK                 0x0203
        #define WM_RBUTTONDOWN                   0x0204
        #define WM_RBUTTONUP                     0x0205
        #define WM_RBUTTONDBLCLK                 0x0206
        #define WM_MBUTTONDOWN                   0x0207
        #define WM_MBUTTONUP                     0x0208
        #define WM_MBUTTONDBLCLK                 0x0209
        #if (_WIN32_WINNT >= 0x0400) || (_WIN32_WINDOWS > 0x0400)
        #define WM_MOUSEWHEEL                    0x020A
        #define WM_MOUSELAST                     0x020A
        #else
        #define WM_MOUSELAST                     0x0209
        #endif /* if (_WIN32_WINNT < 0x0400) * /
        #if(_WIN32_WINNT >= 0x0400)
        #define WHEEL_DELTA                      120      /* Value for rolling one detent * /
        #endif /* _WIN32_WINNT >= 0x0400 * /
        #if(_WIN32_WINNT >= 0x0400)
        #define WHEEL_PAGESCROLL                 (UINT_MAX) /* Scroll one page * /
        #endif /* _WIN32_WINNT >= 0x0400 * /
        #define WM_PARENTNOTIFY                  0x0210
        #define WM_ENTERMENULOOP                 0x0211
        #define WM_EXITMENULOOP                  0x0212
        #if(WINVER >= 0x0400)
        #define WM_NEXTMENU                      0x0213
         // end_r_winuser
        typedef struct tagMDINEXTMENU
         {
              HMENU    hmenuIn;
              HMENU    hmenuNext;
              HWND     hwndNext;
         } MDINEXTMENU, * PMDINEXTMENU, FAR * LPMDINEXTMENU;
        // begin_r_winuser
        #define WM_SIZING                        0x0214
        #define WM_CAPTURECHANGED                0x0215
        #define WM_MOVING                        0x0216
         // end_r_winuser
        #define WM_POWERBROADCAST                0x0218       // r_winuser pbt
         // begin_pbt
        #define PBT_APMQUERYSUSPEND              0x0000
        #define PBT_APMQUERYSTANDBY              0x0001
        #define PBT_APMQUERYSUSPENDFAILED        0x0002
        #define PBT_APMQUERYSTANDBYFAILED        0x0003
        #define PBT_APMSUSPEND                   0x0004
        #define PBT_APMSTANDBY                   0x0005
        #define PBT_APMRESUMECRITICAL            0x0006
        #define PBT_APMRESUMESUSPEND             0x0007
        #define PBT_APMRESUMESTANDBY             0x0008
        #define PBTF_APMRESUMEFROMFAILURE        0x00000001
        #define PBT_APMBATTERYLOW                0x0009
        #define PBT_APMPOWERSTATUSCHANGE         0x000A
        #define PBT_APMOEMEVENT                  0x000B
        #define PBT_APMRESUMEAUTOMATIC           0x0012
         // end_pbt
        // begin_r_winuser
        #define WM_DEVICECHANGE                  0x0219
        #endif /* WINVER >= 0x0400 * /
        #define WM_MDICREATE                     0x0220
        #define WM_MDIDESTROY                    0x0221
        #define WM_MDIACTIVATE                   0x0222
        #define WM_MDIRESTORE                    0x0223
        #define WM_MDINEXT                       0x0224
        #define WM_MDIMAXIMIZE                   0x0225
        #define WM_MDITILE                       0x0226
        #define WM_MDICASCADE                    0x0227
        #define WM_MDIICONARRANGE                0x0228
        #define WM_MDIGETACTIVE                  0x0229
        #define WM_MDISETMENU                    0x0230
        #define WM_ENTERSIZEMOVE                 0x0231
        #define WM_EXITSIZEMOVE                  0x0232
        #define WM_DROPFILES                     0x0233
        #define WM_MDIREFRESHMENU                0x0234
        #if(WINVER >= 0x0400)
        #define WM_IME_SETCONTEXT                0x0281
        #define WM_IME_NOTIFY                    0x0282
        #define WM_IME_CONTROL                   0x0283
        #define WM_IME_COMPOSITIONFULL           0x0284
        #define WM_IME_SELECT                    0x0285
        #define WM_IME_CHAR                      0x0286
        #endif /* WINVER >= 0x0400 * /
        #if(WINVER >= 0x0500)
        #define WM_IME_REQUEST                   0x0288
        #endif /* WINVER >= 0x0500 * /
        #if(WINVER >= 0x0400)
        #define WM_IME_KEYDOWN                   0x0290
        #define WM_IME_KEYUP                     0x0291
        #endif /* WINVER >= 0x0400 * /
        #if(_WIN32_WINNT >= 0x0400)
        #define WM_MOUSEHOVER                    0x02A1
        #define WM_MOUSELEAVE                    0x02A3
        #endif /* _WIN32_WINNT >= 0x0400 * /
        #define WM_CUT                           0x0300
        #define WM_COPY                          0x0301
        #define WM_PASTE                         0x0302
        #define WM_CLEAR                         0x0303
        #define WM_UNDO                          0x0304
        #define WM_RENDERFORMAT                  0x0305
        #define WM_RENDERALLFORMATS              0x0306
        #define WM_DESTROYCLIPBOARD              0x0307
        #define WM_DRAWCLIPBOARD                 0x0308
        #define WM_PAINTCLIPBOARD                0x0309
        #define WM_VSCROLLCLIPBOARD              0x030A
        #define WM_SIZECLIPBOARD                 0x030B
        #define WM_ASKCBFORMATNAME               0x030C
        #define WM_CHANGECBCHAIN                 0x030D
        #define WM_HSCROLLCLIPBOARD              0x030E
        #define WM_QUERYNEWPALETTE               0x030F
        #define WM_PALETTEISCHANGING             0x0310
        #define WM_PALETTECHANGED                0x0311
        #define WM_HOTKEY                        0x0312
        #if(WINVER >= 0x0400)
        #define WM_PRINT                         0x0317
        #define WM_PRINTCLIENT                   0x0318
        #define WM_HANDHELDFIRST                 0x0358
        #define WM_HANDHELDLAST                  0x035F
        #define WM_AFXFIRST                      0x0360
        #define WM_AFXLAST                       0x037F
        #endif /* WINVER >= 0x0400 * /
        #define WM_PENWINFIRST                   0x0380
        #define WM_PENWINLAST                    0x038F
        #if(WINVER >= 0x0400)
        #define WM_APP                           0x8000
        #endif /* WINVER >= 0x0400 * /
         /*
         * NOTE: All Message Numbers below 0x0400 are RESERVED.
         *
         * Private Window Messages Start Here:
         * /
        #define WM_USER                          0x0400
        #if(WINVER >= 0x0400)
        /*   wParam for WM_SIZING message   * /
        #define WMSZ_LEFT            1
        #define WMSZ_RIGHT           2
        #define WMSZ_TOP             3
        #define WMSZ_TOPLEFT         4
        #define WMSZ_TOPRIGHT        5
        #define WMSZ_BOTTOM          6
        #define WMSZ_BOTTOMLEFT      7
        #define WMSZ_BOTTOMRIGHT     8
        #endif /* WINVER >= 0x0400 * /
        #ifndef NONCMESSAGES
        */

        #region Test function
        public static void TestWinApi()
        {
            //TestMehtodEnumWindon();
            //TestMehtodCursor();
            //TestMehtodMessage();
            TestSendInput();
        }

        private static void TestSendInput()
        {
            SendMouse();
            //SendString("1+2*3-4=");
            //SendCtrlC();
        }

        void tempMethod()
        {
            IntPtr calculatorHandle = WinApi.FindWindow("CalcFrame", "Calculator");
            SetForegroundWindow(calculatorHandle);
            var tl = GetWindowTextLength(calculatorHandle);
            var t = new string(' ', tl + 1);
            var ir = GetWindowText(calculatorHandle, t, tl + 1);
            IntPtr p1 = Marshal.AllocHGlobal(256);
            IntPtr p2 = (IntPtr)256;// Marshal.AllocHGlobal(4);
            var p3 = Marshal.StringToHGlobalAnsi("aaa");
            //Marshal.WriteInt32(p2, 256);
            SendMessage(calculatorHandle, 0x000C, (IntPtr)0, p3);//WM_SETTEXT  
            p3 = Marshal.StringToHGlobalAnsi("Calculator");
            SendMessage(calculatorHandle, 0x000C, (IntPtr)0, p3);//WM_SETTEXT  
            Marshal.FreeHGlobal(p1);
            Marshal.FreeHGlobal(p2);
            PostMessage(calculatorHandle, 0x000C, IntPtr.Zero, IntPtr.Zero);//WM_SETTEXT    
            Marshal.FreeHGlobal(p3);
            SendMessage(calculatorHandle, 0x000D, IntPtr.Zero, IntPtr.Zero);//WM_GETTEXT      
            //PostMessage(calculatorHandle, 0x12, 0, 0);//WM_QUIT
            IntPtr consoleWindowClassHandle = WinApi.FindWindow("ConsoleWindowClass", "VS2013 x86 Native Tools Command Prompt");
            return;
            //// Verify that Calculator is a running process.
            //if (calculatorHandle == IntPtr.Zero)
            //{
            //    //MessageBox.Show("Calculator is not running.");
            //    return;
            //}

            //int x = 100; // X coordinate of the click 
            //int y = 80; // Y coordinate of the click 
            //IntPtr handle = calculatorHandle;
            //StringBuilder className = new StringBuilder(100);
            ////while (className.ToString() != "Internet Explorer_Server") // The class control for the browser 
            ////{
            ////    handle = GetWindow(handle, 5); // Get a handle to the child window 
            ////    GetClassName(handle, className, className.Capacity);
            ////}
            //SetForegroundWindow(calculatorHandle);
            //IntPtr childHwnd = FindWindowEx((IntPtr)0x001B074E, IntPtr.Zero, null, "");
            //IntPtr lParam = (IntPtr)((y << 16) | x); // The coordinates 
            //IntPtr wParam = IntPtr.Zero; // Additional parameters for the click (e.g. Ctrl) 
            //SendMessage(handle.ToInt32(), (int)downCode, (int)wParam, lParam.ToString()); // Mouse button down 
            //SendMessage(handle, upCode, wParam, lParam); // Mouse button up 

            // Make Calculator the foreground application and send it 
            // a set of calculations.
            //SetForegroundWindow(calculatorHandle);
            //SendKeys.SendWait("111");
            //SendKeys.SendWait("*");
            //SendKeys.SendWait("11");
            //SendKeys.SendWait("=");

        }
        private static void TestMehtodMessage()
        {
            IntPtr notepadHandle = WinApi.FindWindow("Notepad", "Untitled - Notepad");
            if (notepadHandle != IntPtr.Zero)
            {
                IntPtr editHandle;// = WinApi.FindWindowEx(notepadHandle, IntPtr.Zero, "Edit", "");
                SetForegroundWindow(notepadHandle);
                var sb = new StringBuilder();
                var iPtr = EnumChildWindows(notepadHandle, (h, l) =>
                 {
                     var textLen = GetWindowTextLength(h);
                     string title = new string(' ', textLen + 1);
                     var iResult = GetWindowText(h, title, textLen + 1);
                     var classSb = new StringBuilder();
                     iResult = GetClassName(h, classSb, 256);
                     Console.WriteLine($"Parent:{notepadHandle},Handle:{h},Class:{classSb.ToString()},Title:{title.ToString()}");
                     if (classSb.ToString().Contains("Edit"))
                     {
                         sb.Append(classSb.ToString());
                         editHandle = h;
                     }
                     if (IsWindowVisible(h))
                     {
                         //SetForegroundWindow(h);
                         //System.Threading.Thread.Sleep(1000);
                     }
                     return true;
                 }, 0);

                var p3 = Marshal.StringToHGlobalAnsi(sb.Append("aaabbb").ToString());
                SendMessage(new IntPtr(0x1E0206), 0x000C, (IntPtr)0, p3);//WM_SETTEXT  
            }
        }
        private static void TestMehtodEnumWindon()
        {
            var iPtr = EnumWindows(new WNDENUMPROC(EnumWindowCallBackFunction), 0);
            iPtr = EnumWindows((h, l) =>
            {
                var textLen = GetWindowTextLength(h);
                string title = new string(' ', textLen + 1);
                var iResult = GetWindowText(h, title, textLen + 1);
                Console.WriteLine($"Handle:{h}, Title:{title.ToString()}");
                if (IsWindowVisible(h))
                {
                    //SetForegroundWindow(h);
                    //System.Threading.Thread.Sleep(1000);
                }
                return true;
            }, 0);

            IntPtr calculatorHandle = WinApi.FindWindow("CalcFrame", "Calculator");
            iPtr = EnumChildWindows(calculatorHandle, (h, l) =>
             {
                 var textLen = GetWindowTextLength(h);
                 string title = new string(' ', textLen + 1);
                 var iResult = GetWindowText(h, title, textLen + 1);
                 var sb = new StringBuilder(256);
                 iResult = GetClassName(h, sb, 256);
                 Console.WriteLine($"Parent:{calculatorHandle},Handle:{h},Class:{sb.ToString()},Title:{title.ToString()}");
                 if (IsWindowVisible(h))
                 {
                     //SetForegroundWindow(h);
                     //System.Threading.Thread.Sleep(1000);
                 }
                 return true;
             }, 0);
        }

        public static void SendString(string inputStr)
        {
            //var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            IntPtr hWnd = WinApi.FindWindow("CalcFrame", "Calculator");
            SetForegroundWindow(hWnd);
            List<INPUT> keyList = new List<INPUT>();
            foreach (ushort c in inputStr)
            {
                switch (c)
                {
                    case 8: // Translate \t to VK_TAB
                        {
                            INPUT keyDown = new INPUT();
                            keyDown.Type = 1; //Keyboard
                            keyDown.Data.Keyboard.wVk = (ushort)Keys.Tab;
                            keyDown.Data.Keyboard.dwFlags = 0;
                            keyDown.Data.Keyboard.wScan = 0; //use VirtualKey
                            keyList.Add(keyDown);
                            INPUT keyUp = new INPUT();
                            keyUp.Type = 1; //Keyboard
                            keyUp.Data.Keyboard.wVk = (ushort)Keys.Tab;
                            keyUp.Data.Keyboard.dwFlags = 0x0002;
                            keyUp.Data.Keyboard.wScan = 0; //use VirtualKey
                            keyList.Add(keyUp);
                        }
                        break;
                    case 10: // Translate \n to VK_RETURN
                        {
                            INPUT keyDown = new INPUT();
                            keyDown.Type = 1; //Keyboard
                            keyDown.Data.Keyboard.wVk = (ushort)Keys.Return;
                            keyDown.Data.Keyboard.dwFlags = 0;
                            keyDown.Data.Keyboard.wScan = 0; //use VirtualKey
                            keyList.Add(keyDown);
                            INPUT keyUp = new INPUT();
                            keyUp.Type = 1; //Keyboard
                            keyUp.Data.Keyboard.wVk = (ushort)Keys.Return;
                            keyUp.Data.Keyboard.dwFlags = 0x0002;
                            keyUp.Data.Keyboard.wScan = 0; //use VirtualKey
                            keyList.Add(keyUp);
                        }
                        break;
                    default:
                        {
                            INPUT keyDown = new INPUT();
                            keyDown.Type = 1; //Keyboard
                            keyDown.Data.Keyboard.wVk = 0; //Use unicode
                            keyDown.Data.Keyboard.dwFlags = 0x0004; //Unicode Key Down
                            keyDown.Data.Keyboard.wScan = c;
                            keyList.Add(keyDown);
                            INPUT keyUp = new INPUT();
                            keyUp.Type = 1; //Keyboard
                            keyUp.Data.Keyboard.wVk = 0; //Use unicode
                            keyUp.Data.Keyboard.dwFlags = 0x0004 | 0x0002; //Unicode Key Up
                            keyUp.Data.Keyboard.wScan = c;
                            keyList.Add(keyUp);
                        }
                        break;
                }
            }
            SendInput((uint)keyList.Count, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }
        public static void SendCtrlC()
        {
            List<INPUT> keyList = new List<INPUT>();
            INPUT ctrlKeyDown = new INPUT();
            ctrlKeyDown.Type = 1; //Keyboard
            ctrlKeyDown.Data.Keyboard.wVk = 0; //Use unicode
            ctrlKeyDown.Data.Keyboard.dwFlags = 0x0004; //Unicode Key Down
            ctrlKeyDown.Data.Keyboard.wScan = (ushort)Keys.ControlKey;
            keyList.Add(ctrlKeyDown);
            INPUT cKeyDown = new INPUT();
            cKeyDown.Type = 1; //Keyboard
            cKeyDown.Data.Keyboard.wVk = 0; //Use unicode
            cKeyDown.Data.Keyboard.dwFlags = 0x0004; //Unicode Key Down
            cKeyDown.Data.Keyboard.wScan = (ushort)Keys.ControlKey;
            keyList.Add(cKeyDown);
            INPUT cKeyUp = new INPUT();
            cKeyUp.Type = 1; //Keyboard
            cKeyUp.Data.Keyboard.wVk = 0; //Use unicode
            cKeyUp.Data.Keyboard.dwFlags = 0x0004 | 0x0002; //Unicode Key Up
            cKeyUp.Data.Keyboard.wScan = (ushort)Keys.ControlKey;
            keyList.Add(cKeyUp);
            INPUT ctrlKeyUp = new INPUT();
            ctrlKeyUp.Type = 1; //Keyboard
            ctrlKeyUp.Data.Keyboard.wVk = 0; //Use unicode
            ctrlKeyUp.Data.Keyboard.dwFlags = 0x0004 | 0x0002; //Unicode Key Up
            ctrlKeyUp.Data.Keyboard.wScan = (ushort)Keys.ControlKey;
            keyList.Add(ctrlKeyUp);

            var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            SetForegroundWindow(hWnd);
            SendInput((uint)keyList.Count, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }
        public static void SendMouse()
        {
            var rect = new RECT();
            var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            SetForegroundWindow(hWnd);
            var bResult = GetWindowRect(hWnd, out rect);
            var x = 300;//rect.right > 0 ? rect.right + rect.left / 2 : 100;
            var y = 300;// rect.top > 0 ? rect.top + rect.bottom / 2 : 100;
            Console.WriteLine($"x={x},y={y},left={rect.left},top={rect.top},right={rect.right},bottom={rect.bottom}");
            List<INPUT> keyList = new List<INPUT>();
            INPUT buttonMove = new INPUT();
            buttonMove.Type = 0;//mouse
            buttonMove.Data.Mouse.dx = x;
            buttonMove.Data.Mouse.dy = y;
            buttonMove.Data.Mouse.dwFlags = 0x0001; //MOUSEEVENTF_MOVE
            //buttonMove.Data.Mouse.dwFlags = 0x0001 | 0x8000; //MOUSEEVENTF_MOVE|MOUSEEVENTF_ABSOLUTE
            buttonMove.Data.Mouse.mouseData = 0;
            buttonMove.Data.Mouse.time = 0;
            keyList.Add(buttonMove);

            INPUT buttonDown = new INPUT();
            buttonDown.Type = 0;//mouse
            buttonDown.Data.Mouse.dx = x;
            buttonDown.Data.Mouse.dy = y;
            buttonDown.Data.Mouse.dwFlags = 0x0002; //MOUSEEVENTF_LEFTDOWN
            buttonDown.Data.Mouse.mouseData = 0;
            buttonDown.Data.Mouse.time = 0;
            //keyList.Add(buttonDown);

            INPUT buttonUp = new INPUT();
            buttonUp.Type = 0;//mouse
            buttonUp.Data.Mouse.dx = x;
            buttonUp.Data.Mouse.dy = y;
            buttonUp.Data.Mouse.dwFlags = 0x0004; //MOUSEEVENTF_LEFTUP
            buttonUp.Data.Mouse.mouseData = 0;
            buttonUp.Data.Mouse.time = 0;
            //keyList.Add(buttonUp);

            SendInput((uint)keyList.Count, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }

        private static void TestMehtodCursor()
        {
            var pos = new Point();
            var bResult = GetCursorPos(ref pos);
            if (bResult)
            {
                Console.WriteLine($"X={pos.x},Y={pos.y}");
            }
            IntPtr calculatorHandle = WinApi.FindWindow("CalcFrame", "Calculator");
            SetForegroundWindow(calculatorHandle);
            bResult = SetCursorPos(666, 666);
            bResult = SetCursorPos(743, 115);

            var rect = new RECT();
            // Get a handle to the Calculator application. The window class
            // and window name were obtained using the Spy++ tool.
            bResult = GetWindowRect(calculatorHandle, out rect);
            Console.WriteLine($"{nameof(rect.top)}={rect.top}, {nameof(rect.left)}={rect.left}, {nameof(rect.bottom)}={rect.bottom}, {nameof(rect.right)}={rect.right}");
            //bResult = SetCursorPos(rect.right, rect.top);



            //WinAPI.INPUT keyDown = new WinAPI.INPUT();
            //keyDown.type = 1; //Keyboard
            //keyDown.union.keyboardInput.wVk = 0; //Use unicode
            //keyDown.union.keyboardInput.dwFlags = 0x0004; //Unicode Key Down
            //keyDown.union.keyboardInput.wScan = c;
            //keyList.Add(keyDown);
            //WinAPI.INPUT keyUp = new WinAPI.INPUT();
            //keyUp.type = 1; //Keyboard
            //keyUp.union.keyboardInput.wVk = 0; //Use unicode
            //keyUp.union.keyboardInput.dwFlags = 0x0004 | 0x0002; //Unicode Key Up
            //keyUp.union.keyboardInput.wScan = c;
            //keyList.Add(keyUp);
            //SendInput((uint)keyList.Count, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));


            //INPUT[] InputData = new INPUT[1];
            //InputData[0].Type = (UInt32)InputType.KEYBOARD;
            ////InputData[0].Vk = (ushort)DirectInputKeyScanCode;  //Virtual key is ignored when sending scan code
            //InputData[0].Scan = (ushort)DirectInputKeyScanCode;
            //InputData[0].Flags = (uint)KeyboardFlag.KEYUP | (uint)KeyboardFlag.SCANCODE;
            //InputData[0].Time = 0;
            //InputData[0].ExtraInfo = IntPtr.Zero;
            //// Send Keyup flag "OR"ed with Scancode flag for keyup to work properly
            //SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)))
            //// send keydown
            //if (SendInput(2, InputData, Marshal.SizeOf(InputData[1])) == 0)
            //{
            //    System.Diagnostics.Debug.WriteLine("SendInput failed with code: " +
            //    Marshal.GetLastWin32Error().ToString());
            //}
        }
        private static bool EnumWindowCallBackFunction(IntPtr hwnd, int lParam)
        {
            Console.WriteLine($"Window handle is: {hwnd}");
            return true;
        }
        #endregion  Test function


        public delegate bool WNDENUMPROC(IntPtr hwnd, int lParam);

        /*
        Moves the cursor to the specified screen coordinates. 
        If the new coordinates are not within the screen rectangle set by the most recent ClipCursor function call, 
        the system automatically adjusts the coordinates so that the cursor stays within the rectangle. 

        BOOL WINAPI SetCursorPos(
          _In_ int X,
          _In_ int Y
        );

        Parameters
        X [in]
        Type: int

        The new x-coordinate of the cursor, in screen coordinates. 
        Y [in]
        Type: int

        The new y-coordinate of the cursor, in screen coordinates. 

        Return value

        Type: BOOL

        Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.

        Remarks

        The cursor is a shared resource. A window should move the cursor only when the cursor is in the window's client area.

        The calling process must have WINSTA_WRITEATTRIBUTES access to the window station.

        The input desktop must be the current desktop when you call SetCursorPos. Call OpenInputDesktop to determine whether the current desktop is the input desktop. If it is not, call SetThreadDesktop with the HDESK returned by OpenInputDesktop to switch to that desktop.
        */
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

        /*
        GetCursorPos function

        Retrieves the position of the mouse cursor, in screen coordinates.
        BOOL WINAPI GetCursorPos(
          _Out_ LPPOINT lpPoint
        );

        Parameters
        lpPoint [out]
        Type: LPPOINT

        A pointer to a POINT structure that receives the screen coordinates of the cursor.

        Return value

        Type: BOOL

        Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.

        Remarks

        The cursor position is always specified in screen coordinates and is not affected by the mapping mode of the window that contains the cursor.

        The calling process must have WINSTA_READATTRIBUTES access to the window station.

        The input desktop must be the current desktop when you call GetCursorPos. Call OpenInputDesktop to determine whether the current desktop is the input desktop. If it is not, call SetThreadDesktop with the HDESK returned by OpenInputDesktop to switch to that desktop.
        */
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref Point lpPoint);

        /*
        SendInput function
        Synthesizes keystrokes, mouse motions, and button clicks.
        Syntax
        C++
        Copy
        UINT WINAPI SendInput(
          _In_ UINT    nInputs,
          _In_ LPINPUT pInputs,
          _In_ int     cbSize
        );

        Parameters
        nInputs [in]
        Type: UINT
        The number of structures in the pInputs array.
        pInputs [in]
        Type: LPINPUT
        An array of INPUT structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.
        cbSize [in]
        Type: int
        The size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function fails.
        Return value
        Type: UINT
        The function returns the number of events that it successfully inserted into the keyboard or mouse input stream. If the function returns zero, the input was already blocked by another thread. To get extended error information, call GetLastError.
        This function fails when it is blocked by UIPI. Note that neither GetLastError nor the return value will indicate the failure was caused by UIPI blocking.
        Remarks
        This function is subject to UIPI. Applications are permitted to inject input only into applications that are at an equal or lesser integrity level.
        The SendInput function inserts the events in the INPUT structures serially into the keyboard or mouse input stream. These events are not interspersed with other keyboard or mouse input events inserted either by the user (with the keyboard or mouse) or by calls to keybd_event, mouse_event, or other calls to SendInput.
        This function does not reset the keyboard's current state. Any keys that are already pressed when the function is called might interfere with the events that this function generates. To avoid this problem, check the keyboard's state with the GetAsyncKeyState function and correct as necessary.
        Because the touch keyboard uses the surrogate macros defined in winnls.h to send input to the system, a listener on the keyboard event hook must decode input originating from the touch keyboard. For more information, see Surrogates and Supplementary Characters.
        An accessibility application can use SendInput to inject keystrokes corresponding to application launch shortcut keys that are handled by the shell. This functionality is not guaranteed to work for other types of applications. 
        */
        [DllImport("user32.dll", EntryPoint = "SendInput")]
        public static extern uint SendInput(uint cInputs, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0)] INPUT[] pInputs, int cbSize);

        /*
        GetLastInputInfo function
        Retrieves the time of the last input event.
        Syntax
        C++
        Copy
        BOOL WINAPI GetLastInputInfo(
          _Out_ PLASTINPUTINFO plii
        );
        Parameters
        plii [out]
        Type: PLASTINPUTINFO
        A pointer to a LASTINPUTINFO structure that receives the time of the last input event.
        Return value
        Type: BOOL
        If the function succeeds, the return value is nonzero.
        If the function fails, the return value is zero. 
        Remarks
        This function is useful for input idle detection. However, GetLastInputInfo does not provide system-wide user input information across all running sessions. Rather, GetLastInputInfo provides session-specific user input information for only the session that invoked the function.
        The tick count when the last input event was received (see LASTINPUTINFO) is not guaranteed to be incremental. In some cases, the value might be less than the tick count of a prior event. For example, this can be caused by a timing gap between the raw input thread and the desktop thread or an event raised by SendInput, which supplies its own tick count.
        */
        [DllImport("user32.dll", EntryPoint = "GetLastInputInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetLastInputInfo([Out] out LASTINPUTINFO plii);


        /*
        MapVirtualKey function
        Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code. 
        To specify a handle to the keyboard layout to use for translating the specified code, use the MapVirtualKeyEx function.
        Syntax
        C++
        Copy
        UINT WINAPI MapVirtualKey(
          _In_ UINT uCode,
          _In_ UINT uMapType
        );
        Parameters
        uCode [in]
        Type: UINT
        The virtual key code or scan code for a key. How this value is interpreted depends on the value of the uMapType parameter. 
        uMapType [in]
        Type: UINT
        The translation to be performed. The value of this parameter depends on the value of the uCode parameter. 
        Value
        Meaning
         MAPVK_VK_TO_CHAR2 
        uCode is a virtual-key code and is translated into an unshifted character value in the low-order word of the return value. Dead keys (diacritics) are indicated by setting the top bit of the return value. If there is no translation, the function returns 0.

         MAPVK_VK_TO_VSC0 
        uCode is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If there is no translation, the function returns 0.

         MAPVK_VSC_TO_VK1 
        uCode is a scan code and is translated into a virtual-key code that does not distinguish between left- and right-hand keys. If there is no translation, the function returns 0.

         MAPVK_VSC_TO_VK_EX3 
        uCode is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand keys. If there is no translation, the function returns 0.


        Return value
        Type: UINT
        The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode and uMapType. If there is no translation, the return value is zero.
        Remarks
        An application can use MapVirtualKey to translate scan codes to the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU, and vice versa. These translations do not distinguish between the left and right instances of the SHIFT, CTRL, or ALT keys. 
        An application can get the scan code corresponding to the left or right instance of one of these keys by calling MapVirtualKey with uCode set to one of the following virtual-key code constants. 
        •VK_LSHIFT
        •VK_RSHIFT
        •VK_LCONTROL
        •VK_RCONTROL
        •VK_LMENU
        •VK_RMENU
        These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions.
        */
        [DllImport("user32.dll", EntryPoint = "MapVirtualKey")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        /*
        MapVirtualKeyEx function
        Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code. The function translates the codes using the input language and an input locale identifier.
        Syntax
        C++
        Copy
        UINT WINAPI MapVirtualKeyEx(
        _In_        UINT uCode,
        _In_        UINT uMapType,
        _Inout_opt_ HKL  dwhkl
        );
        Parameters
        uCode [in]
        Type: UINT
        The virtual-key code or scan code for a key. How this value is interpreted depends on the value of the uMapType parameter. 
        Starting with Windows Vista, the high byte of the uCode value can contain either 0xe0 or 0xe1 to specify the extended scan code.
        uMapType [in]
        Type: UINT
        The translation to perform. The value of this parameter depends on the value of the uCode parameter. 
        Value
        Meaning
        MAPVK_VK_TO_CHAR2 
        The uCode parameter is a virtual-key code and is translated into an unshifted character value in the low order word of the return value. Dead keys (diacritics) are indicated by setting the top bit of the return value. If there is no translation, the function returns 0.

        MAPVK_VK_TO_VSC0 
        The uCode parameter is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If there is no translation, the function returns 0.

        MAPVK_VK_TO_VSC_EX4 
        The uCode parameter is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If the scan code is an extended scan code, the high byte of the uCode value can contain either 0xe0 or 0xe1 to specify the extended scan code. If there is no translation, the function returns 0.

        MAPVK_VSC_TO_VK1 
        The uCode parameter is a scan code and is translated into a virtual-key code that does not distinguish between left- and right-hand keys. If there is no translation, the function returns 0. 

        MAPVK_VSC_TO_VK_EX3 
        The uCode parameter is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand keys. If there is no translation, the function returns 0. 


        dwhkl [in, out, optional]
        Type: HKL
        Input locale identifier to use for translating the specified code. This parameter can be any input locale identifier previously returned by the LoadKeyboardLayout function. 
        Return value
        Type: UINT
        The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode and uMapType. If there is no translation, the return value is zero.
        Remarks
        The input locale identifier is a broader concept than a keyboard layout, since it can also encompass a speech-to-text converter, an Input Method Editor (IME), or any other form of input. 
        An application can use MapVirtualKeyEx to translate scan codes to the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU, and vice versa. These translations do not distinguish between the left and right instances of the SHIFT, CTRL, or ALT keys. 
        An application can get the scan code corresponding to the left or right instance of one of these keys by calling MapVirtualKeyEx with uCode set to one of the following virtual-key code constants. 
        •VK_LSHIFT
        •VK_RSHIFT
        •VK_LCONTROL
        •VK_RCONTROL
        •VK_LMENU
        •VK_RMENU
        These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, MapVirtualKey, and MapVirtualKeyEx functions. For list complete table of virtual key codes, see Virtual Key Codes. 
        */
        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyEx")]
        public static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, [In] IntPtr dwhkl);

        /*
        FindWindow function

        Retrieves a handle to the top-level window whose class name and window name match the specified strings. This function does not search child windows. This function does not perform a case-sensitive search.
        To search child windows, beginning with a specified child window, use the FindWindowEx function.
        Syntax
        C++
        Copy
        HWND WINAPI FindWindow(
          _In_opt_ LPCTSTR lpClassName,
          _In_opt_ LPCTSTR lpWindowName
        );
        Parameters
        lpClassName [in, optional]
        Type: LPCTSTR
        The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the high-order word must be zero. 
        If lpClassName points to a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names. 
        If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter. 
        lpWindowName [in, optional]
        Type: LPCTSTR
        The window name (the window's title). If this parameter is NULL, all window names match. 
        Return value
        Type: 
        Type: HWND
        If the function succeeds, the return value is a handle to the window that has the specified class name and window name.
        If the function fails, the return value is NULL. To get extended error information, call GetLastError. 
        Remarks
        If the lpWindowName parameter is not NULL, FindWindow calls the GetWindowText function to retrieve the window name for comparison. For a description of a potential problem that can arise, see the Remarks for GetWindowText. 
        */
        [DllImport("user32.dll", EntryPoint = "FindWindowW")]
        public static extern IntPtr FindWindow([In] [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName);

        /*
        FindWindowEx function
        Retrieves a handle to a window whose class name and window name match the specified strings. The function searches child windows, beginning with the one following the specified child window. This function does not perform a case-sensitive search. 
        Syntax
        C++
        Copy
        HWND WINAPI FindWindowEx(
          _In_opt_ HWND    hwndParent,
          _In_opt_ HWND    hwndChildAfter,
          _In_opt_ LPCTSTR lpszClass,
          _In_opt_ LPCTSTR lpszWindow
        );
        Parameters
        hwndParent [in, optional]
        Type: HWND
        A handle to the parent window whose child windows are to be searched.
        If hwndParent is NULL, the function uses the desktop window as the parent window. The function searches among windows that are child windows of the desktop. 
        If hwndParent is HWND_MESSAGE, the function searches all message-only windows. 
        hwndChildAfter [in, optional]
        Type: HWND
        A handle to a child window. The search begins with the next child window in the Z order. The child window must be a direct child window of hwndParent, not just a descendant window. 
        If hwndChildAfter is NULL, the search begins with the first child window of hwndParent. 
        Note that if both hwndParent and hwndChildAfter are NULL, the function searches all top-level and message-only windows. 
        lpszClass [in, optional]
        Type: LPCTSTR
        The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be placed in the low-order word of lpszClass; the high-order word must be zero.
        If lpszClass is a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names, or it can be MAKEINTATOM(0x8000). In this latter case, 0x8000 is the atom for a menu class. For more information, see the Remarks section of this topic.
        lpszWindow [in, optional]
        Type: LPCTSTR
        The window name (the window's title). If this parameter is NULL, all window names match. 
        Return value
        Type: 
        Type: HWND
        If the function succeeds, the return value is a handle to the window that has the specified class and window names.
        If the function fails, the return value is NULL. To get extended error information, call GetLastError. 
        Remarks
        If the lpszWindow parameter is not NULL, FindWindowEx calls the GetWindowText function to retrieve the window name for comparison. For a description of a potential problem that can arise, see the Remarks section of GetWindowText.
        An application can call this function in the following way.
        FindWindowEx( NULL, NULL, MAKEINTATOM(0x8000), NULL );
        Note that 0x8000 is the atom for a menu class. When an application calls this function, the function checks whether a context menu is being displayed that the application created.
        */
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx([In]IntPtr hWndParent, [In] IntPtr hWndChildAfter, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpszClass, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpszWindow);

        /*
        SendMessage function
        Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To post a message to a thread's message queue and return immediately, use the PostMessage or PostThreadMessage function.
        Syntax
        C++
        Copy
        LRESULT WINAPI SendMessage(
          _In_ HWND   hWnd,
          _In_ UINT   Msg,
          _In_ WPARAM wParam,
          _In_ LPARAM lParam
        );
        Parameters
        hWnd [in]
        Type: HWND
        A handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.
        Message sending is subject to UIPI. The thread of a process can send messages only to message queues of threads in processes of lesser or equal integrity level.
        Msg [in]
        Type: UINT
        The message to be sent.
        For lists of the system-provided messages, see System-Defined Messages.
        wParam [in]
        Type: WPARAM
        Additional message-specific information.
        lParam [in]
        Type: LPARAM
        Additional message-specific information.
        Return value
        Type: 
        Type: LRESULT
        The return value specifies the result of the message processing; it depends on the message sent.
        Remarks
        When a message is blocked by UIPI the last error, retrieved with GetLastError, is set to 5 (access denied).
        Applications that need to communicate using HWND_BROADCAST should use the RegisterWindowMessage function to obtain a unique message for inter-application communication.
        The system only does marshalling for system messages (those in the range 0 to (WM_USER-1)). To send other messages (those >= WM_USER) to another process, you must do custom marshalling.
        If the specified window was created by the calling thread, the window procedure is called immediately as a subroutine. If the specified window was created by a different thread, the system switches to that thread and calls the appropriate window procedure. Messages sent between threads are processed only when the receiving thread executes message retrieval code. The sending thread is blocked until the receiving thread processes the message. However, the sending thread will process incoming nonqueued messages while waiting for its message to be processed. To prevent this, use SendMessageTimeout with SMTO_BLOCK set. For more information on nonqueued messages, see Nonqueued Messages.
        An accessibility application can use SendMessage to send WM_APPCOMMAND messages to the shell to launch applications. This functionality is not guaranteed to work for other types of applications.
        */
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int SendMessage([In] IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /*
        PostMessage function
        Places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message.
        To post a message in the message queue associated with a thread, use the PostThreadMessage function.
        Syntax
        C++
        Copy
        BOOL WINAPI PostMessage(
          _In_opt_ HWND   hWnd,
          _In_     UINT   Msg,
          _In_     WPARAM wParam,
          _In_     LPARAM lParam
        );
        Parameters
        hWnd [in, optional]
        Type: HWND
        A handle to the window whose window procedure is to receive the message. The following values have special meanings.
        Value
        Meaning
         HWND_BROADCAST((HWND)0xffff) 
        The message is posted to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows. The message is not posted to child windows.

        NULL 
        The function behaves like a call to PostThreadMessage with the dwThreadId parameter set to the identifier of the current thread.


        Starting with Windows Vista, message posting is subject to UIPI. The thread of a process can post messages only to message queues of threads in processes of lesser or equal integrity level.
        Msg [in]
        Type: UINT
        The message to be posted.
        For lists of the system-provided messages, see System-Defined Messages.
        wParam [in]
        Type: WPARAM
        Additional message-specific information.
        lParam [in]
        Type: LPARAM
        Additional message-specific information.
        Return value
        Type: 
        Type: BOOL
        If the function succeeds, the return value is nonzero.
        If the function fails, the return value is zero. To get extended error information, call GetLastError. GetLastError returns ERROR_NOT_ENOUGH_QUOTA when the limit is hit. 
        Remarks
        When a message is blocked by UIPI the last error, retrieved with GetLastError, is set to 5 (access denied).
        Messages in a message queue are retrieved by calls to the GetMessage or PeekMessage function.
        Applications that need to communicate using HWND_BROADCAST should use the RegisterWindowMessage function to obtain a unique message for inter-application communication.
        The system only does marshalling for system messages (those in the range 0 to (WM_USER-1)). To send other messages (those >= WM_USER) to another process, you must do custom marshalling.
        If you send a message in the range below WM_USER to the asynchronous message functions (PostMessage, SendNotifyMessage, and SendMessageCallback), its message parameters cannot include pointers. Otherwise, the operation will fail. The functions will return before the receiving thread has had a chance to process the message and the sender will free the memory before it is used.
        Do not post the WM_QUIT message using PostMessage; use the PostQuitMessage function.
        An accessibility application can use PostMessage to post WM_APPCOMMAND messages to the shell to launch applications. This functionality is not guaranteed to work for other types of applications.
        There is a limit of 10,000 posted messages per message queue. This limit should be sufficiently large. If your application exceeds the limit, it should be redesigned to avoid consuming so many system resources. To adjust this limit, modify the following registry key. 
        HKEY_LOCAL_MACHINE
           SOFTWARE
              Microsoft
                 Windows NT
                    CurrentVersion
                       Windows
                          USERPostMessageLimit
        The minimum acceptable value is 4000. 
        */
        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage([In] IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /*
        GetWindowText function
        Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a control in another application.
        Syntax
        C++
        Copy
        int WINAPI GetWindowText(
          _In_  HWND   hWnd,
          _Out_ LPTSTR lpString,
          _In_  int    nMaxCount
        );
        Parameters
        hWnd [in]
        Type: HWND
        A handle to the window or control containing the text. 
        lpString [out]
        Type: LPTSTR
        The buffer that will receive the text. If the string is as long or longer than the buffer, the string is truncated and terminated with a null character. 
        nMaxCount [in]
        Type: int
        The maximum number of characters to copy to the buffer, including the null character. If the text exceeds this limit, it is truncated. 
        Return value
        Type: 
        Type: int
        If the function succeeds, the return value is the length, in characters, of the copied string, not including the terminating null character. If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid, the return value is zero. To get extended error information, call GetLastError. 
        This function cannot retrieve the text of an edit control in another application.
        Remarks
        If the target window is owned by the current process, GetWindowText causes a WM_GETTEXT message to be sent to the specified window or control. If the target window is owned by another process and has a caption, GetWindowText retrieves the window caption text. If the window does not have a caption, the return value is a null string. This behavior is by design. It allows applications to call GetWindowText without becoming unresponsive if the process that owns the target window is not responding. However, if the target window is not responding and it belongs to the calling application, GetWindowText will cause the calling application to become unresponsive. 
        To retrieve the text of a control in another process, send a WM_GETTEXT message directly instead of calling GetWindowText. 

        */
        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowText")]
        public static extern int GetWindowText([In] IntPtr hWnd, [Out] [MarshalAs(UnmanagedType.LPWStr)] string lpString, int nMaxCount);

        /*
    GetWindowTextLength function
    Retrieves the length, in characters, of the specified window's title bar text (if the window has a title bar). If the specified window is a control, the function retrieves the length of the text within the control. However, GetWindowTextLength cannot retrieve the length of the text of an edit control in another application.
    Syntax
    C++
    Copy
    int WINAPI GetWindowTextLength(
      _In_ HWND hWnd
    );
    Parameters
    hWnd [in]
    Type: HWND
    A handle to the window or control.
    Return value
    Type: 
    Type: int
    If the function succeeds, the return value is the length, in characters, of the text. Under certain conditions, this value may actually be greater than the length of the text. For more information, see the following Remarks section.
    If the window has no text, the return value is zero. To get extended error information, call GetLastError. 
    Remarks
    If the target window is owned by the current process, GetWindowTextLength causes a WM_GETTEXTLENGTH message to be sent to the specified window or control. 
    Under certain conditions, the GetWindowTextLength function may return a value that is larger than the actual length of the text. This occurs with certain mixtures of ANSI and Unicode, and is due to the system allowing for the possible existence of double-byte character set (DBCS) characters within the text. The return value, however, will always be at least as large as the actual length of the text; you can thus always use it to guide buffer allocation. This behavior can occur when an application uses both ANSI functions and common dialogs, which use Unicode. It can also occur when an application uses the ANSI version of GetWindowTextLength with a window whose window procedure is Unicode, or the Unicode version of GetWindowTextLength with a window whose window procedure is ANSI. For more information on ANSI and ANSI functions, see Conventions for Function Prototypes. 
    To obtain the exact length of the text, use the WM_GETTEXT, LB_GETTEXT, or CB_GETLBTEXT messages, or the GetWindowText function.

    */
        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength")]
        public static extern int GetWindowTextLength([In] IntPtr hWnd);

        /*
        GetClassName function
        Retrieves the name of the class to which the specified window belongs. 
        Syntax
        C++
        Copy
        int WINAPI GetClassName(
          _In_  HWND   hWnd,
          _Out_ LPTSTR lpClassName,
          _In_  int    nMaxCount
        );
        Parameters
        hWnd [in]
        Type: HWND
        A handle to the window and, indirectly, the class to which the window belongs. 
        lpClassName [out]
        Type: LPTSTR
        The class name string.
        nMaxCount [in]
        Type: int
        The length of the lpClassName buffer, in characters. The buffer must be large enough to include the terminating null character; otherwise, the class name string is truncated to nMaxCount-1 characters.
        Return value
        Type: 
        Type: int
        If the function succeeds, the return value is the number of characters copied to the buffer, not including the terminating null character.
        If the function fails, the return value is zero. To get extended error information, call GetLastError. 

        */
        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetClassName")]
        public static extern int GetClassName([In] IntPtr hWnd, [Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpClassName, int nMaxCount);

        /*
        EnumWindows function

        Enumerates all top-level windows on the screen by passing the handle to each window, in turn, to an application-defined callback function. EnumWindows continues until the last top-level window is enumerated or the callback function returns FALSE. 
        Syntax
        C++
        Copy
        BOOL WINAPI EnumWindows(
          _In_ WNDENUMPROC lpEnumFunc,
          _In_ LPARAM      lParam
        );
        Parameters
        lpEnumFunc [in]
        Type: WNDENUMPROC
        A pointer to an application-defined callback function. For more information, see EnumWindowsProc. 
        lParam [in]
        Type: LPARAM
        An application-defined value to be passed to the callback function. 
        Return value
        Type: 
        Type: BOOL
        If the function succeeds, the return value is nonzero.
        If the function fails, the return value is zero. To get extended error information, call GetLastError.
        If EnumWindowsProc returns zero, the return value is also zero. In this case, the callback function should call SetLastError to obtain a meaningful error code to be returned to the caller of EnumWindows.
        Remarks
        The EnumWindows function does not enumerate child windows, with the exception of a few top-level windows owned by the system that have the WS_CHILD style.
        This function is more reliable than calling the GetWindow function in a loop. An application that calls GetWindow to perform this task risks being caught in an infinite loop or referencing a handle to a window that has been destroyed. 
        Note  For Windows 8 and later, EnumWindows enumerates only top-level windows of desktop apps.
        */
        [DllImport("user32.dll", EntryPoint = "EnumWindows")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, [In] [MarshalAs(UnmanagedType.I4)] int lParam);

        /*
        EnumChildWindows function
        Enumerates the child windows that belong to the specified parent window by passing the handle to each child window, in turn, to an application-defined callback function. EnumChildWindows continues until the last child window is enumerated or the callback function returns FALSE.
        Syntax
        C++
        Copy
        BOOL WINAPI EnumChildWindows(
          _In_opt_ HWND        hWndParent,
          _In_     WNDENUMPROC lpEnumFunc,
          _In_     LPARAM      lParam
        );
        Parameters
        hWndParent [in, optional]
        Type: HWND
        A handle to the parent window whose child windows are to be enumerated. If this parameter is NULL, this function is equivalent to EnumWindows. 
        lpEnumFunc [in]
        Type: WNDENUMPROC
        A pointer to an application-defined callback function. For more information, see EnumChildProc. 
        lParam [in]
        Type: LPARAM
        An application-defined value to be passed to the callback function. 
        Return value
        Type: 
        Type: BOOL
        The return value is not used.
        Remarks
        If a child window has created child windows of its own, EnumChildWindows enumerates those windows as well. 
        A child window that is moved or repositioned in the Z order during the enumeration process will be properly enumerated. The function does not enumerate a child window that is destroyed before being enumerated or that is created during the enumeration process. 
        */
        [DllImport("user32.dll", EntryPoint = "EnumChildWindows")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows([In] IntPtr hWndParent, WNDENUMPROC lpEnumFunc, [In] [MarshalAs(UnmanagedType.I4)] int lParam);

        /*
        IsWindowVisible function
        Determines the visibility state of the specified window. 
        Syntax
        C++
        Copy
        BOOL WINAPI IsWindowVisible(
          _In_ HWND hWnd
        );
        Parameters
        hWnd [in]
        Type: HWND
        A handle to the window to be tested. 
        Return value
        Type: 
        Type: BOOL
        If the specified window, its parent window, its parent's parent window, and so forth, have the WS_VISIBLE style, the return value is nonzero. Otherwise, the return value is zero. 
        Because the return value specifies whether the window has the WS_VISIBLE style, it may be nonzero even if the window is totally obscured by other windows. 
        Remarks
        The visibility state of a window is indicated by the WS_VISIBLE style bit. When WS_VISIBLE is set, the window is displayed and subsequent drawing into it is displayed as long as the window has the WS_VISIBLE style. 
        Any drawing to a window with the WS_VISIBLE style will not be displayed if the window is obscured by other windows or is clipped by its parent window. 
        */
        [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible([In] IntPtr hWnd);

        /*
        ShowWindow function
        Sets the specified window's show state. 
        Syntax
        C++
        Copy
        BOOL WINAPI ShowWindow(
          _In_ HWND hWnd,
          _In_ int  nCmdShow
        );
        Parameters
        hWnd [in]
        Type: HWND
        A handle to the window. 
        nCmdShow [in]
        Type: int
        Controls how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of the following values. 
        Value
        Meaning
         SW_FORCEMINIMIZE11 
        Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.

         SW_HIDE0 
        Hides the window and activates another window.

         SW_MAXIMIZE3 
        Maximizes the specified window.

         SW_MINIMIZE6 
        Minimizes the specified window and activates the next top-level window in the Z order.

         SW_RESTORE9 
        Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.

         SW_SHOW5 
        Activates the window and displays it in its current size and position. 

         SW_SHOWDEFAULT10 
        Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application. 

         SW_SHOWMAXIMIZED3 
        Activates the window and displays it as a maximized window.

         SW_SHOWMINIMIZED2 
        Activates the window and displays it as a minimized window.

         SW_SHOWMINNOACTIVE7 
        Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.

         SW_SHOWNA8 
        Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.

         SW_SHOWNOACTIVATE4 
        Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.

         SW_SHOWNORMAL1 
        Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.


        Return value
        Type: 
        Type: BOOL
        If the window was previously visible, the return value is nonzero. 
        If the window was previously hidden, the return value is zero. 
        Remarks
        To perform certain special effects when showing or hiding a window, use AnimateWindow. 
        The first time an application calls ShowWindow, it should use the WinMain function's nCmdShow parameter as its nCmdShow parameter. Subsequent calls to ShowWindow must use one of the values in the given list, instead of the one specified by the WinMain function's nCmdShow parameter. 
        As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in the first call to ShowWindow if the program that launched the application specifies startup information in the structure. In this case, ShowWindow uses the information specified in the STARTUPINFO structure to show the window. On subsequent calls, the application must call ShowWindow with nCmdShow set to SW_SHOWDEFAULT to use the startup information provided by the program that launched the application. This behavior is designed for the following situations: 
        •Applications create their main window by calling CreateWindow with the WS_VISIBLE flag set. 
        •Applications create their main window by calling CreateWindow with the WS_VISIBLE flag cleared, and later call ShowWindow with the SW_SHOW flag set to make it visible. 
        Examples
        */
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow([In] IntPtr hWnd, int nCmdShow);

        /*
        SetForegroundWindow function
        Brings the thread that created the specified window into the foreground and activates the window. Keyboard input is directed to the window, and various visual cues are changed for the user. The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads. 
        Syntax
        C++
        Copy
        BOOL WINAPI SetForegroundWindow(
          _In_ HWND hWnd
        );
        Parameters
        hWnd [in]
        Type: HWND
        A handle to the window that should be activated and brought to the foreground. 
        Return value
        Type: 
        Type: BOOL
        If the window was brought to the foreground, the return value is nonzero. 
        If the window was not brought to the foreground, the return value is zero.
        Remarks
        The system restricts which processes can set the foreground window. A process can set the foreground window only if one of the following conditions is true: 
        •The process is the foreground process.
        •The process was started by the foreground process.
        •The process received the last input event.
        •There is no foreground process.
        •The process is being debugged.
        •The foreground process is not a Modern Application or the Start Screen.
        •The foreground is not locked (see LockSetForegroundWindow).
        •The foreground lock time-out has expired (see SPI_GETFOREGROUNDLOCKTIMEOUT in SystemParametersInfo).
        •No menus are active.
        An application cannot force a window to the foreground while the user is working with another window. Instead, Windows flashes the taskbar button of the window to notify the user.
        A process that can set the foreground window can enable another process to set the foreground window by calling the AllowSetForegroundWindow function. The process specified by dwProcessId loses the ability to set the foreground window the next time the user generates input, unless the input is directed at that process, or the next time a process calls AllowSetForegroundWindow, unless that process is specified. 
        The foreground process can disable calls to SetForegroundWindow by calling the LockSetForegroundWindow function. 

        */
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow([In] IntPtr hWnd);//don't show the window if it is min to taskbar

        /*
        GetWindowRect function
        Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
        Syntax
        C++
        Copy
        BOOL WINAPI GetWindowRect(
          _In_  HWND   hWnd,
          _Out_ LPRECT lpRect
        );
        Parameters
        hWnd [in]
        Type: HWND
        A handle to the window. 
        lpRect [out]
        Type: LPRECT
        A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window. 
        Return value
        Type: 
        Type: BOOL
        If the function succeeds, the return value is nonzero.
        If the function fails, the return value is zero. To get extended error information, call GetLastError. 
        Remarks
        In conformance with conventions for the RECT structure, the bottom-right coordinates of the returned rectangle are exclusive. In other words, the pixel at (right, bottom) lies immediately outside the rectangle.

        */
        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect([In] IntPtr hWnd, [Out] out RECT lpRect);

        /*
        ScreenToClient function
        The ScreenToClient function converts the screen coordinates of a specified point on the screen to client-area coordinates.
        Syntax
        C++
        Copy
        BOOL ScreenToClient(
          _In_ HWND    hWnd,
               LPPOINT lpPoint
        );
        Parameters
        hWnd [in]
        A handle to the window whose client area will be used for the conversion.
        lpPoint 
        A pointer to a POINT structure that specifies the screen coordinates to be converted.
        Return value
        If the function succeeds, the return value is nonzero.
        If the function fails, the return value is zero.
        Remarks
        The function uses the window identified by the hWnd parameter and the screen coordinates given in the POINT structure to compute client coordinates. It then replaces the screen coordinates with the client coordinates. The new coordinates are relative to the upper-left corner of the specified window's client area.
        The ScreenToClient function assumes the specified point is in screen coordinates.
        All coordinates are in device units.
        Do not use ScreenToClient when in a mirroring situation, that is, when changing from left-to-right layout to right-to-left layout. Instead, use MapWindowPoints. For more information, see "Window Layout and Mirroring" in Window Features.
        */
        [DllImport("user32.dll", EntryPoint = "ScreenToClient")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ScreenToClient([In] IntPtr hWnd, ref Point lpPoint);

        /*GetClientRect function
        Retrieves the coordinates of a window's client area. The client coordinates specify the upper-left and lower-right corners of the client area. Because client coordinates are relative to the upper-left corner of a window's client area, the coordinates of the upper-left corner are (0,0). 
        Syntax
        C++
        Copy
        BOOL WINAPI GetClientRect(
          _In_  HWND   hWnd,
          _Out_ LPRECT lpRect
        );
        Parameters
        hWnd [in]
        Type: HWND
        A handle to the window whose client coordinates are to be retrieved. 
        lpRect [out]
        Type: LPRECT
        A pointer to a RECT structure that receives the client coordinates. The left and top members are zero. The right and bottom members contain the width and height of the window. 
        Return value
        Type: 
        Type: BOOL
        If the function succeeds, the return value is nonzero.
        If the function fails, the return value is zero. To get extended error information, call GetLastError. 
        Remarks
        In conformance with conventions for the RECT structure, the bottom-right coordinates of the returned rectangle are exclusive. In other words, the pixel at (right, bottom) lies immediately outside the rectangle.
        */
        [DllImport("user32.dll", EntryPoint = "GetClientRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect([In] IntPtr hWnd, [Out] out RECT lpRect);

        /*
        PostQuitMessage function
        Indicates to the system that a thread has made a request to terminate (quit). It is typically used in response to a WM_DESTROY message.
        Syntax
        C++
        Copy
        VOID WINAPI PostQuitMessage(
          _In_ int nExitCode
        );
        Parameters
        nExitCode [in]
        Type: int
        The application exit code. This value is used as the wParam parameter of the WM_QUIT message.
        Return value
        This function does not return a value.
        Remarks
        The PostQuitMessage function posts a WM_QUIT message to the thread's message queue and returns immediately; the function simply indicates to the system that the thread is requesting to quit at some time in the future. 
        When the thread retrieves the WM_QUIT message from its message queue, it should exit its message loop and return control to the system. The exit value returned to the system must be the wParam parameter of the WM_QUIT message.

        */
        [DllImport("user32.dll", EntryPoint = "PostQuitMessage")]
        public static extern void PostQuitMessage(int nExitCode);

        //Note  This function has been superseded. Use SendInput instead.
        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);

        //Note  This function has been superseded. Use SendInput instead.
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private static void DoMouseClick(int x, int y)
        {
            //int dx = (int)((double)x / Screen.PrimaryScreen.Bounds.Width * 65535); //屏幕分辨率映射到0~65535(0xffff,即16位)之间
            //int dy = (int)((double)y / Screen.PrimaryScreen.Bounds.Height * 0xffff); //转换为double类型运算，否则值为0、1
            //mouse_event(MouseEventFlag.Move | MouseEventFlag.LeftDown | MouseEventFlag.LeftUp | MouseEventFlag.Absolute, dx, dy, 0, new UIntPtr(0)); //点击
            //keybd_event((byte)Keys.ControlKey, 0, 0, 0);
            //keybd_event((byte)Keys.C, 0, 0, 0);
            //keybd_event((byte)Keys.ControlKey, 0, 2, 0);
            //keybd_event((byte)Keys.C, 0, 2, 0);

        }
        // Send a series of key presses to the Calculator application.
        private void button1_Click(object sender, EventArgs e)
        {
            // Get a handle to the Calculator application. The window class
            // and window name were obtained using the Spy++ tool.
            IntPtr calculatorHandle = FindWindow("CalcFrame", "Calculator");

            // Verify that Calculator is a running process.
            if (calculatorHandle == IntPtr.Zero)
            {
                //MessageBox.Show("Calculator is not running.");
                return;
            }

            // Make Calculator the foreground application and send it 
            // a set of calculations.
            SetForegroundWindow(calculatorHandle);
            //SendKeys.SendWait("111");
            //SendKeys.SendWait("*");
            //SendKeys.SendWait("11");
            //SendKeys.SendWait("=");
        }
    }
}


//鼠标移动绘制十字测量线就得用异或线而不是Paint消息

//        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

//DoubleBuffered = true;

//public void Show(System.Windows.Forms.Control control)
//02.{  
//03.    Graphics gc = control.CreateGraphics();  
//04.    // 创建缓冲图形上下文 （类似 Win32 中的CreateCompatibleDC）  
//05.    BufferedGraphicsContext dc = new BufferedGraphicsContext();   
//06.    // 创建指定大小缓冲区 （类似 Win32 中的 CreateCompatibleBitmap）  
//07.    BufferedGraphics backBuffer = dc.Allocate(gc, new Rectangle(new Point(0, 0), control.Size));       
//08.              
//09.    /* 像使用一般的 Graphics 一样绘图 */  
//10.    Pen pen = new Pen(Color.Gray);  
//11.    foreach (Step s in m_steps)  
//12.    {  
//13.        gc.DrawLine(pen, s.Start, s.End);  
//14.    }  
//15.      
//16.    // 将双缓冲区中的图形渲染到指定画布上 （类似 Win32 中的）BitBlt  
//17.    backBuffer.Render(control.CreateGraphics());     
//18.}  




//public class DoubleBufferPanel : Panel

//{

//    public DoubleBufferPanel()

//    {

//        // Set the value of the double-buffering style bits to true. 

//        this.DoubleBuffered = true;

//        this.SetStyle(ControlStyles.AllPaintingInWmPaint |

//        ControlStyles.UserPaint |

//        ControlStyles.OptimizedDoubleBuffer, true);

//        UpdateStyles();

//    }

//}



//Dim g As Graphics
//    Private Sub Form1_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
//        Dim point As Point = PointToClient(MousePosition)
//        Dim x As Integer = point.X
//        Dim y As Integer = point.Y
//        g.Clear(Me.BackColor) '更多.net源码和教程，来自[乐博网 www.lob.cn]
//        g.DrawLine(Pens.Black, 0, y, Me.Width, y)
//        g.DrawLine(Pens.Black, x, 0, x, Me.Height)
//    End Sub




//1.4 个参数的意义是一样的，返回值类型不同（其实从数据上看他们一样是一个 32 位的数，只是意义不一样），LRESULT 表示的是消息被处理后的返回值，BOOL 表示的是消息是不是Post 成功。


//2.PostMessage 是异步的，SendMessage 是同步的。PostMessage 只把消息放入队列，不管消息是否被处理就返回，消息可能不被处理；而SendMessage 等待消息被处理完了之后才返回，如果消息不被处理，发送消息的线程将一直被阻塞。


//3.如果在同一个线程内，SendMessage 发送消息时，由 USER32.DLL 模块调用目标窗口的消息处理程序，并将结果返回。SendMessage 在同一线程中发送消息并不入线程消息队列。PostMessage 发送消息时，消息要先放入线程的消息队列，然后通过消息循环分派到目标窗口（DispatchMessage）。


//4.如果在不同线程内，SendMessage 发送消息到目标窗口所属线程的消息队列，然后发送消息的线程在USER32.DLL 模块内监视和等待消息处理，直到目标窗口处理完返回。SendMessage 在返回前还做了很多工作，比如，响应别的线程向它SendMessage。Post 到别的线程时，最好用PostThreadMessage 代替 PostMessage，PostMessage 的 hWnd 参数可以是 NULL，等效于 PostThreadMessage + GetCurrentThreadId。Post WM_QUIT 时，应使用 PostQuitMessage 代替。


//2、PostMessage 是异步的，SendMessage 是同步的。
//PostMessage 只把消息放入队列，不管消息是否被处理就返回，消息可能不被处理；而 SendMessage 等待消息被处理完了之后才返回，如果消息不被处理，发送消息的线程将一直被阻塞。


//3、如果在同一个线程内，SendMessage 发送消息时，由 USER32.DLL
//模块调用目标窗口的消息处理程序，并将结果返回。SendMessage 在同一线程中发送消息并不入线程消息队列。PostMessage
//发送消息时，消息要先放入线程的消息队列，然后通过消息循环分派到目标窗口（DispatchMessage）。


// 如果在不同线程内，SendMessage 发送消息到目标窗口所属线程的消息队列，然后发送消息的线程在 USER32.DLL
//模块内监视和等待消息处理，直到目标窗口处理完返回。SendMessage 在返回前还做了很多工作，比如，响应别的线程向它
//SendMessage。Post 到别的线程时，最好用 PostThreadMessage 代替
//PostMessage，PostMessage 的 hWnd 参数可以是 NULL，等效于 PostThreadMessage +
// GetCurrentThreadId。Post WM_QUIT 时，应使用 PostQuitMessage 代替。

//4、系统只整编（marshal）系统消息（0 到 WM_USER 之间的消息），发送用户消息（WM_USER 以上）到别的进程时，需要自己做整编。

// 用 PostMessage、SendNotifyMessage、SendMessageCallback 等异步函数发送系统消息时，参数里不可以使用指针，因为发送者并不等待消息的处理就返回，接受者还没处理指针就已经被释放了。 5、在 Windows 2000/XP 里，每个消息队列最多只能存放 10,000 个 Post 的消息，超过的还没被处理的将不会被处理，直接丢掉。这个值可以改得更大：[HKEY_LOCAL_MACHINE/SOFTWARE/ Microsoft/Windows NT/CurrentVersion/Windows] USERPostMessageLimit，最小可以是 4000。 PostMessage只负责将消息放到消息队列中，不确定何时及是否处理 SendMessage要等到受到消息处理的返回码（DWord类型）后才继续 PostMessage执行后马上返回 SendMessage必须等到消息被处理后才会返回。



//  private void btnMouseClick_Click(object sender, EventArgs e)
//{
//    int x = 100; // X coordinate of the click 
//    int y = 80; // Y coordinate of the click 
//    IntPtr handle = webBrowser1.Handle;
//    StringBuilder className = new StringBuilder(100);
//    while (className.ToString() != "Internet Explorer_Server") // The class control for the browser 
//    {
//        handle = GetWindow(handle, 5); // Get a handle to the child window 
//        GetClassName(handle, className, className.Capacity);
//    }

//    IntPtr lParam = (IntPtr)((y << 16) | x); // The coordinates 
//    IntPtr wParam = IntPtr.Zero; // Additional parameters for the click (e.g. Ctrl) 
//    const uint downCode = 0x201; // Left click down code 
//    const uint upCode = 0x202; // Left click up code 
//    SendMessage(handle, downCode, wParam, lParam); // Mouse button down 
//    SendMessage(handle, upCode, wParam, lParam); // Mouse button up 
//}



//[DllImport("user32.dll", EntryPoint = "FindWindow")]
//public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

//[DllImport("user32.dll", EntryPoint = "FindWindowEx")]
//private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

//[DllImport("user32.dll", EntryPoint = "SendMessage")]
//private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

//private void closeWindows()
//{
//    const int BM_CLICK = 0xF5;
//    IntPtr maindHwnd = FindWindow(null, "保存文件"); //获得程序句柄
//    if (maindHwnd != IntPtr.Zero)
//    {
//        IntPtr childHwnd = FindWindowEx(maindHwnd, IntPtr.Zero, null, "否(&N)"); //获得按钮的句柄
//        if (childHwnd != IntPtr.Zero)
//        {
//            SendMessage(childHwnd, BM_CLICK, 0, 0); //发送点击按钮的消息
//        }
//        else
//        {
//            MessageBox.Show("没有找到子窗口");
//        }
//    }
//    else
//    {
//        MessageBox.Show("没有找到窗口");
//    }
//}

