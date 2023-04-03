//////////////////////////////////////////////////////////////////////////////////////////
////                                      modbus.c                                    ////
////                                                                                  ////
////                 MODBUS protocol driver for serial communications.                ////
////                                                                                  ////
////  Refer to documentation at http://www.modbus.org for more information on MODBUS. ////
////                                                                                  ////
//////////////////////////////////////////////////////////////////////////////////////////
////                                                                                  ////
//// DEFINES:                                                                         ////
////                                                                                  ////
////  MODBUS_TYPE                   MODBUS_TYPE_MASTER or MODBUS_TYPE_SLAVE           ////
////  MODBUS_SERIAL_INT_SOURCE      Source of interrupts                              ////
////                                (MODBUS_INT_EXT,MODBUS_INT_RDA,MODBUS_INT_RDA2,   ////
////                                   MODBUS_INT_RDA3,MODBUS_INT_RDA4)               ////
////  MODBUS_SERIAL_TYPE            MODBUS_RTU or MODBUS_ASCII                        ////
////  MODBUS_SERIAL_BAUD            Valid baud rate for serial                        ////
////  MODBUS_SERIAL_RX_PIN          Valid pin for serial receive                      ////
////  MODBUS_SERIAL_TX_PIN          Valid pin for serial transmit                     ////
////  MODBUS_SERIAL_ENABLE_PIN      Valid pin for serial enable, rs485 only           ////
////  MODBUS_SERIAL_RX_ENABLE       Valid pin for serial rcv enable, rs485 only       ////
////  MODBUS_SERAIL_RX_BUFFER_SIZE  Size of the receive buffer                        ////
////                                                                                  ////
////                                                                                  ////
//// SHARED API:                                                                      ////
////                                                                                  ////
////  modbus_init()                                                                   ////
////    - Initialize modbus serial communication system                               ////
////                                                                                  ////
////  modbus_serial_send_start(address,func)                                          ////
////    - Setup serial line to begin sending.  Once this is called, you can send data ////
////      using modbus_serial_putc().  Should only be used for custom commands.       ////
////                                                                                  ////
////  modbus_serial_send_stop()                                                       ////
////    - Must be called to finalize the send when modbus_serial_send_start is used.  ////
////                                                                                  ////
////  modbus_kbhit()                                                                  ////
////    - Used to check if a packet has been received.                                ////
////                                                                                  ////
//// MASTER API:                                                                      ////
////  All master API functions return 0 on success.                                   ////
////                                                                                  ////
////  exception modbus_read_coils(address,start_address,quantity)                     ////
////    - Wrapper for function 0x01(read coils) in the MODBUS specification.          ////
////                                                                                  ////
////  exception modbus_read_discrete_input(address,start_address,quantity)            ////
////    - Wrapper for function 0x02(read discret input) in the MODBUS specification.  ////
////                                                                                  ////
////  exception modbus_read_holding_registers(address,start_address,quantity)         ////
////    - Wrapper for function 0x03(read holding regs) in the MODBUS specification.   ////
////                                                                                  ////
////  exception modbus_read_input_registers(address,start_address,quantity)           ////
////    - Wrapper for function 0x04(read input regs) in the MODBUS specification.     ////
////                                                                                  ////
////  exception modbus_write_single_coil(address,output_address,on)                   ////
////    - Wrapper for function 0x05(write single coil) in the MODBUS specification.   ////
////                                                                                  ////
////  exception modbus_write_single_register(address,reg_address,reg_value)           ////
////    - Wrapper for function 0x06(write single reg) in the MODBUS specification.    ////
////                                                                                  ////
////  exception modbus_read_exception_status(address)                                 ////
////    - Wrapper for function 0x07(read void status) in the MODBUS specification.    ////
////                                                                                  ////
////  exception modbus_diagnostics(address,sub_func,data)                             ////
////    - Wrapper for function 0x08(diagnostics) in the MODBUS specification.         ////
////                                                                                  ////
////  exception modbus_get_comm_event_counter(address)                                ////
////    - Wrapper for function 0x0B(get comm event count) in the MODBUS specification.////
////                                                                                  ////
////  exception modbus_get_comm_event_log(address)                                    ////
////    - Wrapper for function 0x0C(get comm event log) in the MODBUS specification.  ////
////                                                                                  ////
////  exception modbus_write_multiple_coils(address,start_address,quantity,*values)   ////
////    - Wrapper for function 0x0F(write multiple coils) in the MODBUS specification.////
////    - Special Note: values is a pointer to an int8 array, each byte represents 8  ////
////                    coils.                                                        ////
////                                                                                  ////
////  exception modbus_write_multiple_registers(address,start_address,quantity,*values)///
////    - Wrapper for function 0x10(write multiple regs) in the MODBUS specification. ////
////    - Special Note: values is a pointer to an int8 array                          ////
////                                                                                  ////
////  exception modbus_report_slave_id(address)                                       ////
////    - Wrapper for function 0x11(report slave id) in the MODBUS specification.     ////
////                                                                                  ////
////  exception modbus_read_file_record(address,byte_count,*request)                  ////
////    - Wrapper for function 0x14(read file record) in the MODBUS specification.    ////
////                                                                                  ////
////  exception modbus_write_file_record(address,byte_count,*request)                 ////
////    - Wrapper for function 0x15(write file record) in the MODBUS specification.   ////
////                                                                                  ////
////  exception modbus_mask_write_register(address,reference_address,AND_mask,OR_mask)////
////    - Wrapper for function 0x16(read coils) in the MODBUS specification.          ////
////                                                                                  ////
////  exception modbus_read_write_multiple_registers(address,read_start,read_quantity,////
////                            write_start,write_quantity, *write_registers_value)   ////
////    - Wrapper for function 0x17(read write mult regs) in the MODBUS specification.////
////                                                                                  ////
////  exception modbus_read_FIFO_queue(address,FIFO_address)                          ////
////    - Wrapper for function 0x18(read FIFO queue) in the MODBUS specification.     ////
////                                                                                  ////
////                                                                                  ////
//// Slave API:                                                                       ////
////                                                                                  ////
////  void modbus_read_coils_rsp(address,byte_count,*coil_data)                       ////
////    - Wrapper to respond to 0x01(read coils) in the MODBUS specification.         ////
////                                                                                  ////
////  void modbus_read_discrete_input_rsp(address,byte_count,*input_data)             ////
////    - Wrapper to respond to 0x02(read discret input) in the MODBUS specification. ////
////                                                                                  ////
////  void modbus_read_holding_registers_rsp(address,byte_count,*reg_data)            ////
////    - Wrapper to respond to 0x03(read holding regs) in the MODBUS specification.  ////
////                                                                                  ////
////  void modbus_read_input_registers_rsp(address,byte_count,*input_data)            ////
////    - Wrapper to respond to 0x04(read input regs) in the MODBUS specification.    ////
////                                                                                  ////
////  void modbus_write_single_coil_rsp(address,output_address,output_value)          ////
////    - Wrapper to respond to 0x05(write single coil) in the MODBUS specification.  ////
////                                                                                  ////
////  void modbus_write_single_register_rsp(address,reg_address,reg_value)            ////
////    - Wrapper to respond to 0x06(write single reg) in the MODBUS specification.   ////
////                                                                                  ////
////  void modbus_read_exception_status_rsp(address, data)                            ////
////    - Wrapper to respond to 0x07(read void status) in the MODBUS specification.   ////
////                                                                                  ////
////  void modbus_diagnostics_rsp(address,sub_func,data)                              ////
////    - Wrapper to respond to 0x08(diagnostics) in the MODBUS specification.        ////
////                                                                                  ////
////  void modbus_get_comm_event_counter_rsp(address,status,event_count)              ////
////    - Wrapper to respond to 0x0B(get comm event count) in the MODBUS specification////
////                                                                                  ////
////  void modbus_get_comm_event_log_rsp(address,status,event_count,message_count,    ////
////                                   *events, events_len)                           ////
////    - Wrapper to respond to 0x0C(get comm event log) in the MODBUS specification. ////
////                                                                                  ////
////  void modbus_write_multiple_coils_rsp(address,start_address,quantity)            ////
////    - Wrapper to respond to 0x0F(write multiple coils) in the MODBUS specification////
////                                                                                  ////
////  void modbus_write_multiple_registers_rsp(address,start_address,quantity)        ////
////    - Wrapper to respond to 0x10(write multiple regs) in the MODBUS specification.////
////                                                                                  ////
////  void modbus_report_slave_id_rsp(address,slave_id,run_status,*data,data_len)     ////
////    - Wrapper to respond to 0x11(report slave id) in the MODBUS specification.    ////
////                                                                                  ////
////  void modbus_read_file_record_rsp(address,byte_count,*request)                   ////
////    - Wrapper to respond to 0x14(read file record) in the MODBUS specification.   ////
////                                                                                  ////
////  void modbus_write_file_record_rsp(address,byte_count,*request)                  ////
////    - Wrapper to respond to 0x15(write file record) in the MODBUS specification.  ////
////                                                                                  ////
////  void modbus_mask_write_register_rsp(address,reference_address,AND_mask,OR_mask) ////
////    - Wrapper to respond to 0x16(read coils) in the MODBUS specification.         ////
////                                                                                  ////
////  void modbus_read_write_multiple_registers_rsp(address,*data,data_len)           ////
////    - Wrapper to respond to 0x17(read write mult regs) in the MODBUS specification////
////                                                                                  ////
////  void modbus_read_FIFO_queue_rsp(address,FIFO_len,*data)                         ////
////    - Wrapper to respond to 0x18(read FIFO queue) in the MODBUS specification.    ////
////                                                                                  ////
////  void modbus_exception_rsp(int8 address, int16 func, exception error)            ////
////    - Wrapper to send an exception response.  See exception list below.           ////
////                                                                                  ////
//// Exception List:                                                                  ////
////  ILLEGAL_FUNCTION, ILLEGAL_DATA_ADDRESS, ILLEGAL_DATA_VALUE,                     ////
////  SLAVE_DEVICE_FAILURE, ACKNOWLEDGE, SLAVE_DEVICE_BUSY, MEMORY_PARITY_ERROR,      ////
////  GATEWAY_PATH_UNAVAILABLE, GATEWAY_TARGET_NO_RESPONSE                            ////
////                                                                                  ////
//////////////////////////////////////////////////////////////////////////////////////////
////                                                                                  ////
//// Revision history:                                                                ////
////  May 8, 2009       Made PCD Compatible                                           ////
////  August 21, 2009   Added Modbus ASCII protocol                                   ////
////                                                                                  ////
//////////////////////////////////////////////////////////////////////////////////////////
////                (C) Copyright 1996, 2006 Custom Computer Services                 ////
////        This source code may only be used by licensed users of the CCS            ////
////        C compiler.  This source code may only be distributed to other            ////
////        licensed users of the CCS C compiler.  No other use,                      ////
////        reproduction or distribution is permitted without written                 ////
////        permission.  Derivative programs created using this software              ////
////        in object code form are not restricted in any way.                        ////
//////////////////////////////////////////////////////////////////////////////////////////

/*Some defines so we can use identifiers to set things up*/
#define MODBUS_TYPE_MASTER 99999
#define MODBUS_TYPE_SLAVE  88888
#define MODBUS_INT_RDA     77777
#define MODBUS_INT_RDA2    66666
#define MODBUS_INT_RDA3    44444
#define MODBUS_INT_RDA4    33333
#define MODBUS_INT_EXT     55555
#define MODBUS_RTU         1
#define MODBUS_ASCII       2

#ifndef MODBUS_TYPE
#define MODBUS_TYPE MODBUS_TYPE_MASTER
#endif

#ifndef MODBUS_SERIAL_TYPE
#define MODBUS_SERIAL_TYPE MODBUS_RTU
#endif

#ifndef MODBUS_SERIAL_INT_SOURCE
#define MODBUS_SERIAL_INT_SOURCE MODBUS_INT_RDA    // Select between external interrupt
#endif                                             // or asynchronous serial interrupt

#ifndef MODBUS_SERIAL_BAUD
#define MODBUS_SERIAL_BAUD 9600
#endif

#ifndef MODBUS_SERIAL_RX_PIN
#define MODBUS_SERIAL_RX_PIN       PIN_C7   // Data receive pin
#endif

#ifndef MODBUS_SERIAL_TX_PIN
#define MODBUS_SERIAL_TX_PIN       PIN_C6   // Data transmit pin
#endif

#ifndef MODBUS_SERIAL_ENABLE_PIN
#define MODBUS_SERIAL_ENABLE_PIN   0   // Controls DE pin.  RX low, TX high.
#endif

#ifndef MODBUS_SERIAL_RX_ENABLE
#define MODBUS_SERIAL_RX_ENABLE    0   // Controls RE pin.  Should keep low.
#endif

#ifndef MODBUS_SERIAL_TIMEOUT
   #if (MODBUS_SERIAL_TYPE == MODBUS_ASCII)
      #define MODBUS_SERIAL_TIMEOUT    1000000
   #else
      #define MODBUS_SERIAL_TIMEOUT      10000     //in us
   #endif
#endif

#if( MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA )
   #use rs232(baud=MODBUS_SERIAL_BAUD, UART1, bits=8, stop=1, parity=N, stream=MODBUS_SERIAL, errors)
   #define RCV_OFF() {disable_interrupts(INT_RDA);}
#elif( MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA2 )
   #use rs232(baud=MODBUS_SERIAL_BAUD, UART2, bits=8, stop=1, parity=N, stream=MODBUS_SERIAL, errors)
   #define RCV_OFF() {disable_interrupts(INT_RDA2);}
#elif( MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA3 )
   #use rs232(baud=MODBUS_SERIAL_BAUD, UART3, bits=8, stop=1, parity=N, stream=MODBUS_SERIAL, errors)
   #define RCV_OFF() {disable_interrupts(INT_RDA3);}
#elif( MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA4 )
   #use rs232(baud=MODBUS_SERIAL_BAUD, UART4, bits=8, stop=1, parity=N, stream=MODBUS_SERIAL, errors)
   #define RCV_OFF() {disable_interrupts(INT_RDA4);}
#elif( MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_EXT )
   #use rs232(baud=MODBUS_SERIAL_BAUD, xmit=MODBUS_SERIAL_TX_PIN, rcv=MODBUS_SERIAL_RX_PIN, bits=8, stop=1, parity=N, stream=MODBUS_SERIAL, disable_ints)
   #if defined(__PCD__)
   #define RCV_OFF() {disable_interrupts(INT_EXT0);}
   #else
   #define RCV_OFF() {disable_interrupts(INT_EXT);}
   #endif
#else
   #error Please define a correct interrupt source
#endif

#ifndef MODBUS_SERIAL_RX_BUFFER_SIZE
#define MODBUS_SERIAL_RX_BUFFER_SIZE  64      //size of send/rcv buffer
#endif

#if (MODBUS_TYPE == MODBUS_TYPE_MASTER)
int32 modbus_serial_wait=MODBUS_SERIAL_TIMEOUT;

#define MODBUS_SERIAL_WAIT_FOR_RESPONSE()\
{\
    if(address)\
    {\
        while(!modbus_kbhit() && --modbus_serial_wait)\
            delay_us(1);\
        if(!modbus_serial_wait)\
            modbus_rx.error=TIMEOUT;\
    }\
    modbus_serial_wait = MODBUS_SERIAL_TIMEOUT;\
}
#endif

#if (MODBUS_SERIAL_INT_SOURCE != MODBUS_INT_EXT)
   #if defined(__PCD__)
      #if (MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA)
         #word TXSTA=getenv("SFR:U1STA") 
         #bit TRMT=TXSTA.8
      #elif (MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA2)
         #word TXSTA=getenv("SFR:U2STA")
         #bit TRMT=TXSTA.8
      #elif (MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA3)
         #word TXSTA=getenv("SFR:U3STA")
         #bit TRMT=TXSTA.8
      #elif (MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA4)
         #word TXSTA=getenv("SFR:U4STA")
         #bit TRMT=TXSTA.8
      #endif
   #else
      #if (MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA)
         #if getenv("sfr:TXSTA")
            #byte TXSTA=getenv("sfr:TXSTA")
         #else
            #byte TXSTA=getenv("sfr:TXSTA1")
         #endif
      #elif (MODBUS_SERIAL_INT_SOURCE == MODBUS_INT_RDA2)
         #byte TXSTA=getenv("sfr:TXSTA2")
      #endif
      #bit TRMT=TXSTA.1
   #endif

#define WAIT_FOR_HW_BUFFER()\
{\
   while(!TRMT);\
}   
#endif

int1 modbus_serial_new=0;

/********************************************************************
These exceptions are defined in the MODBUS protocol.  These can be
used by the slave to communicate problems with the transmission back
to the master who can also use these to easily check the exceptions.  
The first exception is the only one that is not part of the protocol 
specification.  The TIMEOUT exception is returned when no slave 
responds to the master's request within the timeout period.
********************************************************************/
typedef enum _exception{ILLEGAL_FUNCTION=1,ILLEGAL_DATA_ADDRESS=2, 
ILLEGAL_DATA_VALUE=3,SLAVE_DEVICE_FAILURE=4,ACKNOWLEDGE=5,SLAVE_DEVICE_BUSY=6, 
MEMORY_PARITY_ERROR=8,GATEWAY_PATH_UNAVAILABLE=10,GATEWAY_TARGET_NO_RESPONSE=11,
TIMEOUT=12} exception;

/********************************************************************
These functions are defined in the MODBUS protocol.  These can be
used by the slave to check the incomming function.  See 
ex_modbus_slave.c for example usage.
********************************************************************/
typedef enum _function{FUNC_READ_COILS=0x01,FUNC_READ_DISCRETE_INPUT=0x02,
FUNC_READ_HOLDING_REGISTERS=0x03,FUNC_READ_INPUT_REGISTERS=0x04,
FUNC_WRITE_SINGLE_COIL=0x05,FUNC_WRITE_SINGLE_REGISTER=0x06,
FUNC_READ_EXCEPTION_STATUS=0x07,FUNC_DIAGNOSTICS=0x08,
FUNC_GET_COMM_EVENT_COUNTER=0x0B,FUNC_GET_COMM_EVENT_LOG=0x0C,
FUNC_WRITE_MULTIPLE_COILS=0x0F,FUNC_WRITE_MULTIPLE_REGISTERS=0x10,
FUNC_REPORT_SLAVE_ID=0x11,FUNC_READ_FILE_RECORD=0x14,
FUNC_WRITE_FILE_RECORD=0x15,FUNC_MASK_WRITE_REGISTER=0x16,
FUNC_READ_WRITE_MULTIPLE_REGISTERS=0x17,FUNC_READ_FIFO_QUEUE=0x18} function;
    
/*Stages of MODBUS reception.  Used to keep our ISR fast enough.*/
#if (MODBUS_SERIAL_TYPE==MODBUS_ASCII)
   enum {MODBUS_START=0, MODBUS_GETADDY, MODBUS_GETFUNC, MODBUS_GETDATA, MODBUS_STOP} modbus_serial_state=0;
#else
   enum {MODBUS_GETADDY=0, MODBUS_GETFUNC=1, MODBUS_GETDATA=2} modbus_serial_state = 0;
#endif

/*Global value holding our current CRC value.*/
#if (MODBUS_SERIAL_TYPE==MODBUS_ASCII)
   unsigned int8 modbus_serial_lrc;
#else
   union
   {
      int8 b[2];
      int16 d;
   } modbus_serial_crc;
#endif

/********************************************************************
Our receive struct.  This is used when receiving data as a master or
slave.  Once a message is sent to you with your address, you should
begin processing that message.  Refer to ex_modbus_slave.c to see 
how to properly use this structure.
********************************************************************/
struct
{
   int8 address;
   int8 len;                                //number of bytes in the message received
   function func;                           //the function of the message received
   exception error;                         //error recieved, if any
   int8 data[MODBUS_SERIAL_RX_BUFFER_SIZE]; //data of the message received
} modbus_rx;

/* Table of CRC values for high–order byte */
#if (MODBUS_SERIAL_TYPE == MODBUS_RTU)
const unsigned char modbus_auchCRCHi[] = {
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,
0x40,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,
0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,
0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,
0x40,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,
0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,
0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,
0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,
0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,
0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,
0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,
0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,
0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,
0x40
};

/* Table of CRC values for low–order byte */
const char modbus_auchCRCLo[] = {
0x00,0xC0,0xC1,0x01,0xC3,0x03,0x02,0xC2,0xC6,0x06,0x07,0xC7,0x05,0xC5,0xC4,
0x04,0xCC,0x0C,0x0D,0xCD,0x0F,0xCF,0xCE,0x0E,0x0A,0xCA,0xCB,0x0B,0xC9,0x09,
0x08,0xC8,0xD8,0x18,0x19,0xD9,0x1B,0xDB,0xDA,0x1A,0x1E,0xDE,0xDF,0x1F,0xDD,
0x1D,0x1C,0xDC,0x14,0xD4,0xD5,0x15,0xD7,0x17,0x16,0xD6,0xD2,0x12,0x13,0xD3,
0x11,0xD1,0xD0,0x10,0xF0,0x30,0x31,0xF1,0x33,0xF3,0xF2,0x32,0x36,0xF6,0xF7,
0x37,0xF5,0x35,0x34,0xF4,0x3C,0xFC,0xFD,0x3D,0xFF,0x3F,0x3E,0xFE,0xFA,0x3A,
0x3B,0xFB,0x39,0xF9,0xF8,0x38,0x28,0xE8,0xE9,0x29,0xEB,0x2B,0x2A,0xEA,0xEE,
0x2E,0x2F,0xEF,0x2D,0xED,0xEC,0x2C,0xE4,0x24,0x25,0xE5,0x27,0xE7,0xE6,0x26,
0x22,0xE2,0xE3,0x23,0xE1,0x21,0x20,0xE0,0xA0,0x60,0x61,0xA1,0x63,0xA3,0xA2,
0x62,0x66,0xA6,0xA7,0x67,0xA5,0x65,0x64,0xA4,0x6C,0xAC,0xAD,0x6D,0xAF,0x6F,
0x6E,0xAE,0xAA,0x6A,0x6B,0xAB,0x69,0xA9,0xA8,0x68,0x78,0xB8,0xB9,0x79,0xBB,
0x7B,0x7A,0xBA,0xBE,0x7E,0x7F,0xBF,0x7D,0xBD,0xBC,0x7C,0xB4,0x74,0x75,0xB5,
0x77,0xB7,0xB6,0x76,0x72,0xB2,0xB3,0x73,0xB1,0x71,0x70,0xB0,0x50,0x90,0x91,
0x51,0x93,0x53,0x52,0x92,0x96,0x56,0x57,0x97,0x55,0x95,0x94,0x54,0x9C,0x5C,
0x5D,0x9D,0x5F,0x9F,0x9E,0x5E,0x5A,0x9A,0x9B,0x5B,0x99,0x59,0x58,0x98,0x88,
0x48,0x49,0x89,0x4B,0x8B,0x8A,0x4A,0x4E,0x8E,0x8F,0x4F,0x8D,0x4D,0x4C,0x8C,
0x44,0x84,0x85,0x45,0x87,0x47,0x46,0x86,0x82,0x42,0x43,0x83,0x41,0x81,0x80,
0x40
};
#endif

// Purpose:    Enable data reception
// Inputs:     None
// Outputs:    None
void RCV_ON(void)
{
   #if (MODBUS_SERIAL_INT_SOURCE!=MODBUS_INT_EXT)
      while(kbhit(MODBUS_SERIAL)) {getc();}  //Clear RX buffer. Clear RDA interrupt flag. Clear overrun error flag.
      #if (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA)
        clear_interrupt(INT_RDA);
      #elif (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA2)
        clear_interrupt(INT_RDA2);
      #elif (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA3)
        clear_interrupt(INT_RDA3);
      #else
        clear_interrupt(INT_RDA4);
      #endif

      #if (MODBUS_SERIAL_RX_ENABLE!=0) 
         output_low(MODBUS_SERIAL_RX_ENABLE);
      #endif

      #if (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA)
        enable_interrupts(INT_RDA);
      #elif (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA2)
        enable_interrupts(INT_RDA2);
      #elif (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA3)
        enable_interrupts(INT_RDA3);
      #else
        enable_interrupts(INT_RDA4);
      #endif
   #else
      #if defined(__PCD__)
         clear_interrupt(INT_EXT0);
      #else
         clear_interrupt(INT_EXT);
      #endif
      
      ext_int_edge(H_TO_L);
     
      #if (MODBUS_SERIAL_RX_ENABLE!=0) 
         output_low(MODBUS_SERIAL_RX_ENABLE);
      #endif

      #if defined(__PCD__)
         enable_interrupts(INT_EXT0);
      #else
         enable_interrupts(INT_EXT);
      #endif
   #endif
}

// Purpose:    Initialize RS485 communication. Call this before
//             using any other RS485 functions.
// Inputs:     None
// Outputs:    None
void modbus_init()
{
   output_low(MODBUS_SERIAL_ENABLE_PIN);

   RCV_ON();

   #if defined(__PCD__)
      #if (MODBUS_SERIAL_TYPE == MODBUS_RTU)
         setup_timer2(TMR_INTERNAL | TMR_DIV_BY_8,4999); //~4ms interrupts for 20Mhz clock
      #endif
      enable_interrupts(INTR_GLOBAL);
   #else
      #if (MODBUS_SERIAL_TYPE == MODBUS_RTU)
         setup_timer_2(T2_DIV_BY_16,249,5);  //~4ms interrupts
      #endif
      enable_interrupts(GLOBAL);
   #endif
}

// Purpose:    Start our timeout timer
// Inputs:     Enable, used to turn timer on/off
// Outputs:    None
// Not used for ASCII mode
#if (MODBUS_SERIAL_TYPE == MODBUS_RTU)
void modbus_enable_timeout(int1 enable)
{
   disable_interrupts(INT_TIMER2);
   if (enable) {
      set_timer2(0);
      clear_interrupt(INT_TIMER2);
      enable_interrupts(INT_TIMER2);
   }
}
#endif

// Purpose:    Check if we have timed out waiting for a response
// Inputs:     None
// Outputs:    None
// Not used for ASCII mode
#if (MODBUS_SERIAL_TYPE == MODBUS_RTU)
   #int_timer2
   void modbus_timeout_now(void)
   {
      if((modbus_serial_state == MODBUS_GETDATA) && (modbus_serial_crc.d == 0x0000) && (!modbus_serial_new))
      {
         modbus_rx.len-=2;
         modbus_serial_new=TRUE;
      }
      else
         modbus_serial_new=FALSE;
   
      modbus_serial_crc.d=0xFFFF;
      modbus_serial_state=MODBUS_GETADDY;
      modbus_enable_timeout(FALSE);
   }
#endif

// Purpose:    Calculate crc of data and updates global crc
// Inputs:     Character
// Outputs:    None
void modbus_calc_crc(char data)
{
   #if (MODBUS_SERIAL_TYPE == MODBUS_ASCII)
      modbus_serial_lrc+=data;
   #else
      unsigned int8 uIndex ; // will index into CRC lookup table

      uIndex = (modbus_serial_crc.b[1]) ^ data; // calculate the CRC
      modbus_serial_crc.b[1] = (modbus_serial_crc.b[0]) ^ modbus_auchCRCHi[uIndex];
      modbus_serial_crc.b[0] = modbus_auchCRCLo[uIndex];
   #endif
}

// Purpose:    Puts a character onto the serial line
// Inputs:     Character
// Outputs:    None
void modbus_serial_putc(int8 c)
{
   #if (MODBUS_SERIAL_TYPE==MODBUS_ASCII)
      int8 asciih,asciil;
      
      asciih=c>>4;
      if(asciih>9)
         asciih+=0x37;
      else
         asciih+=0x30;
      asciil=c&0xF;
      if(asciil>9)
         asciil+=0x37;
      else
         asciil+=0x30;
      fputc(asciih,MODBUS_SERIAL);
      fputc(asciil,MODBUS_SERIAL);
      modbus_calc_crc(c);
   #else
      fputc(c, MODBUS_SERIAL);
      modbus_calc_crc(c);
      delay_us(1000000/MODBUS_SERIAL_BAUD); //one stop bit.  not exact
   #endif
}

// Purpose:   Interrupt service routine for handling incoming serial data
// Inputs:    None
// Outputs:   None
//Khai bao ngat cho viec nhan du lieu tu Master-->Slave hoac nguoc lai.
//Khi ngat la RDA thi Master la PC

#if (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA)
#int_rda
#elif (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA2)
#int_rda2
#elif (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA3)
#int_rda3
#elif (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_RDA4)
#int_rda4
#elif (MODBUS_SERIAL_INT_SOURCE==MODBUS_INT_EXT)
#if defined(__PCD__)
#int_ext0
#else
#int_ext
#endif
#else
#error Please define a correct interrupt source
#endif
void incomming_modbus_serial() {
   char c;
   #if (MODBUS_SERIAL_TYPE==MODBUS_ASCII)
      static int1 two_characters=0;
      static int8 datah,datal,data;
   #endif

   c=fgetc(MODBUS_SERIAL);
   
   if (!modbus_serial_new)
   {
      #if (MODBUS_SERIAL_TYPE==MODBUS_ASCII)
         if(modbus_serial_state == MODBUS_START)
         {
            if(c==':')
               modbus_serial_state++;
         }
         else if(modbus_serial_state == MODBUS_GETADDY)
         {
            if(!two_characters)
            {
               if(c>=0x41)
                  datah=((c-0x37)<<4);
               else
                  datah=((c-0x30)<<4);
               modbus_serial_lrc=0;
            }
            else
            {
               if(c>=0x41)
                  datal=c-0x37;
               else
                  datal=c-0x30;
               data=(datah | datal);
               modbus_rx.address=data;
               modbus_calc_crc(data);
               modbus_serial_state++;
            }
            two_characters++;
         }
         else if(modbus_serial_state == MODBUS_GETFUNC)
         {
            if(!two_characters)
            {
               if(c>=0x41)
                  datah=((c-0x37)<<4);
               else
                  datah=((c-0x30)<<4);
            }
            else
            {
               if(c>=0x41)
                  datal=c-0x37;
               else
                  datal=c-0x30;
               data=(datah | datal);
               modbus_rx.func=data;
               modbus_calc_crc(data);
               modbus_serial_state++;
               modbus_rx.len=0;
               modbus_rx.error=0;
            }
            two_characters++;
         }
         else if(modbus_serial_state == MODBUS_GETDATA)
         {
            if(c=='\r')
            {
               modbus_serial_state++;
               modbus_rx.len--;
               modbus_serial_lrc-=data;
            }
            else if(!two_characters)
            {
               if(c>=0x41)
                  datah=((c-0x37)<<4);
               else
                  datah=((c-0x30)<<4);
               two_characters++;
            }
            else
            {
               if(c>=0x41)
                  datal=c-0x37;
               else
                  datal=c-0x30;
               data=(datah | datal);
               if (modbus_rx.len>=MODBUS_SERIAL_RX_BUFFER_SIZE)
                  modbus_rx.len=MODBUS_SERIAL_RX_BUFFER_SIZE-1;
               modbus_rx.data[modbus_rx.len]=data;
               modbus_rx.len++;
               modbus_calc_crc(data);
               two_characters++;
            }
         }
         else if(modbus_serial_state==MODBUS_STOP)
         {
            if(c=='\n')
            {
               modbus_serial_lrc=((0xFF-modbus_serial_lrc)+1);
               if(modbus_serial_lrc==data)
                  modbus_serial_new=TRUE;
            }
            modbus_serial_state=MODBUS_START;
            two_characters=0;
         }
      #else
            
         if(modbus_serial_state == MODBUS_GETADDY)
         {
            modbus_serial_crc.d = 0xFFFF;
            modbus_rx.address = c;
            modbus_serial_state++;
            modbus_rx.len = 0;
            modbus_rx.error=0;
         }
         else if(modbus_serial_state == MODBUS_GETFUNC)
         {
            modbus_rx.func = c;
            modbus_serial_state++;
         }
         else if(modbus_serial_state == MODBUS_GETDATA)
         {
            if (modbus_rx.len>=MODBUS_SERIAL_RX_BUFFER_SIZE) {modbus_rx.len=MODBUS_SERIAL_RX_BUFFER_SIZE-1;}
            modbus_rx.data[modbus_rx.len]=c;
            modbus_rx.len++;
         }
   
         modbus_calc_crc(c);
         modbus_enable_timeout(TRUE);
      #endif
   }
   #if (MODBUS_TYPE == MODBUS_TYPE_MASTER)
      modbus_serial_wait=MODBUS_SERIAL_TIMEOUT;
   #endif
}

// Purpose:    Send a message over the RS485 bus
// Inputs:     1) The destination address
//             2) The number of bytes of data to send
//             3) A pointer to the data to send
//             4) The length of the data
// Outputs:    TRUE if successful
//             FALSE if failed
// Note:       Format:  source | destination | data-length | data | checksum
void modbus_serial_send_start(int8 to, int8 func)
{
   #if (MODBUS_SERIAL_TYPE==MODBUS_ASCII)
      modbus_serial_lrc=0;
   #else
      modbus_serial_crc.d=0xFFFF;
   #endif
   modbus_serial_new=FALSE;

   RCV_OFF();
   
#if (MODBUS_SERIAL_ENABLE_PIN!=0) 
   output_high(MODBUS_SERIAL_ENABLE_PIN);
#endif

   #if (MODBUS_SERIAL_TYPE==MODBUS_RTU)
      delay_us(3500000/MODBUS_SERIAL_BAUD); //3.5 character delay
   #else
      fputc(':',MODBUS_SERIAL);
   #endif

   modbus_serial_putc(to);
   modbus_serial_putc(func);
}

void modbus_serial_send_stop()
{
   #if (MODBUS_SERIAL_TYPE == MODBUS_ASCII)
      int8 i;
      
      for(i=0;i<8;i++)
      {
         if(bit_test(modbus_serial_lrc,i))
            bit_clear(modbus_serial_lrc,i);
         else
            bit_set(modbus_serial_lrc,i);
      }
      modbus_serial_lrc++;
      
      modbus_serial_putc(modbus_serial_lrc);
      fputc('\r',MODBUS_SERIAL);
      fputc('\n',MODBUS_SERIAL);
   #else
      int8 crc_low, crc_high;
   
      crc_high=modbus_serial_crc.b[1];
      crc_low=modbus_serial_crc.b[0];
   
      modbus_serial_putc(crc_high);
      modbus_serial_putc(crc_low);
   #endif
   
#if (MODBUS_SERIAL_INT_SOURCE!=MODBUS_INT_EXT)
   WAIT_FOR_HW_BUFFER();
#endif
   
   #if (MODBUS_SERIAL_TYPE == MODBUS_RTU)
      delay_us(3500000/MODBUS_SERIAL_BAUD); //3.5 character delay
   #endif

   RCV_ON();

#if (MODBUS_SERIAL_ENABLE_PIN!=0) 
   output_low(MODBUS_SERIAL_ENABLE_PIN);
#endif

   #if (MODBUS_SERIAL_TYPE == MODBUS_ASCII)
      modbus_serial_lrc=0;
   #else
      modbus_serial_crc.d=0xFFFF;
   #endif
}

// Purpose:    Get a message from the RS485 bus and store it in a buffer
// Inputs:     None
// Outputs:    TRUE if a message was received
//             FALSE if no message is available
// Note:       Data will be filled in at the modbus_rx struct:
int1 modbus_kbhit()
{
   if(!modbus_serial_new)
      return FALSE;
   else if(modbus_rx.func & 0x80)           //did we receive an error?
   {
      modbus_rx.error = modbus_rx.data[0];  //if so grab the error and return true
      modbus_rx.len = 1;
   }
   modbus_serial_new=FALSE;
   return TRUE;
}

#if (MODBUS_TYPE==MODBUS_TYPE_MASTER)
/*MODBUS Master Functions*/

/********************************************************************
The following structs are used for read/write_sub_request.  These
functions take in one of these structs.
Please refer to the MODBUS protocol specification if you do not
understand the members of the structure.
********************************************************************/
typedef struct _modbus_read_sub_request
{
   int8 reference_type;
   int16 file_number;
   int16 record_number;
   int16 record_length;
} modbus_read_sub_request;

typedef struct _modbus_write_sub_request
{
   int8 reference_type;
   int16 file_number;
   int16 record_number;
   int16 record_length;
   int16 data[MODBUS_SERIAL_RX_BUFFER_SIZE-8];
} modbus_write_sub_request;


/********************************************************************
The following functions are defined in the MODBUS protocol.  Please
refer to http://www.modbus.org for the purpose of each of these.
All functions take the slaves address as their first parameter.
Each function returns the exception code received from the response.
The function will return 0 if there were no errors in transmission.
********************************************************************/

/*
read_coils
Input:     int8       address            Slave Address
           int16      start_address      Address to start reading from
           int16      quantity           Amount of addresses to read
Output:    exception                     0 if no error, else the exception
*/
exception modbus_read_coils(int8 address, int16 start_address, int16 quantity)
{
   modbus_serial_send_start(address, FUNC_READ_COILS);

   modbus_serial_putc(make8(start_address,1));
   modbus_serial_putc(make8(start_address,0));

   modbus_serial_putc(make8(quantity,1));
   modbus_serial_putc(make8(quantity,0));

   modbus_serial_send_stop();
   
   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
read_discrete_input
Input:     int8       address            Slave Address
           int16      start_address      Address to start reading from
           int16      quantity           Amount of addresses to read
Output:    exception                     0 if no error, else the exception
*/
exception modbus_read_discrete_input(int8 address, int16 start_address, int16 quantity)
{
   modbus_serial_send_start(address, FUNC_READ_DISCRETE_INPUT);

   modbus_serial_putc(make8(start_address,1));
   modbus_serial_putc(make8(start_address,0));

   modbus_serial_putc(make8(quantity,1));
   modbus_serial_putc(make8(quantity,0));

   modbus_serial_send_stop();
      
   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
read_holding_registers
Input:     int8       address            Slave Address
           int16      start_address      Address to start reading from
           int16      quantity           Amount of addresses to read
Output:    exception                     0 if no error, else the exception
*/
exception modbus_read_holding_registers(int8 address, int16 start_address, int16 quantity)
{
   modbus_serial_send_start(address, FUNC_READ_HOLDING_REGISTERS);

   modbus_serial_putc(make8(start_address,1));
   modbus_serial_putc(make8(start_address,0));

   modbus_serial_putc(make8(quantity,1));
   modbus_serial_putc(make8(quantity,0));

   modbus_serial_send_stop();
   
   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
read_input_registers
Input:     int8       address            Slave Address
           int16      start_address      Address to start reading from
           int16      quantity           Amount of addresses to read
Output:    exception                     0 if no error, else the exception
*/
exception modbus_read_input_registers(int8 address, int16 start_address, int16 quantity)
{
   modbus_serial_send_start(address, FUNC_READ_INPUT_REGISTERS);

   modbus_serial_putc(make8(start_address,1));
   modbus_serial_putc(make8(start_address,0));

   modbus_serial_putc(make8(quantity,1));
   modbus_serial_putc(make8(quantity,0));

   modbus_serial_send_stop();
   
   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
write_single_coil
Input:     int8       address            Slave Address
           int16      output_address     Address to write into
           int1       on                 true for on, false for off
Output:    exception                     0 if no error, else the exception
*/
exception modbus_write_single_coil(int8 address, int16 output_address, int1 on)
{
   modbus_serial_send_start(address, FUNC_WRITE_SINGLE_COIL);

   modbus_serial_putc(make8(output_address,1));
   modbus_serial_putc(make8(output_address,0));

   if(on)
       modbus_serial_putc(0xFF);
   else
       modbus_serial_putc(0x00);
   
   modbus_serial_putc(0x00);

   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
write_single_register
Input:     int8       address            Slave Address
           int16      reg_address        Address to write into
           int16      reg_value          Value to write
Output:    exception                     0 if no error, else the exception
*/
exception modbus_write_single_register(int8 address, int16 reg_address, int16 reg_value)
{
   modbus_serial_send_start(address, FUNC_WRITE_SINGLE_REGISTER);

   modbus_serial_putc(make8(reg_address,1));
   modbus_serial_putc(make8(reg_address,0));

   modbus_serial_putc(make8(reg_value,1));
   modbus_serial_putc(make8(reg_value,0));

   modbus_serial_send_stop();
   
   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
read_exception_status
Input:     int8       address            Slave Address
Output:    exception                     0 if no error, else the exception
*/
exception modbus_read_exception_status(int8 address)
{
   modbus_serial_send_start(address, FUNC_READ_EXCEPTION_STATUS);
   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
diagnostics
Input:     int8       address            Slave Address
           int16      sub_func           Subfunction to send
           int16      data               Data to send, changes based on subfunction
Output:    exception                     0 if no error, else the exception
*/
exception modbus_diagnostics(int8 address, int16 sub_func, int16 data)
{
   modbus_serial_send_start(address, FUNC_DIAGNOSTICS);

   modbus_serial_putc(make8(sub_func,1));
   modbus_serial_putc(make8(sub_func,0));

   modbus_serial_putc(make8(data,1));
   modbus_serial_putc(make8(data,0));

   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
get_comm_event_couter
Input:     int8       address            Slave Address
Output:    exception                     0 if no error, else the exception
*/
exception modbus_get_comm_event_counter(int8 address)
{
   modbus_serial_send_start(address, FUNC_GET_COMM_EVENT_COUNTER);
   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
get_comm_event_log
Input:     int8       address            Slave Address
Output:    exception                     0 if no error, else the exception
*/
exception modbus_get_comm_event_log(int8 address)
{
   modbus_serial_send_start(address, FUNC_GET_COMM_EVENT_LOG);
   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
write_multiple_coils
Input:     int8       address            Slave Address
           int16      start_address      Address to start at
           int16      quantity           Amount of coils to write to
           int1*      values             A pointer to an array holding the values to write
Output:    exception                     0 if no error, else the exception
*/
exception modbus_write_multiple_coils(int8 address, int16 start_address, int16 quantity,
                           int8 *values)
{
   int8 i,count;
   
   count = (int8)((quantity/8));
   
   if(quantity%8)
      count++;      

   modbus_serial_send_start(address, FUNC_WRITE_MULTIPLE_COILS);

   modbus_serial_putc(make8(start_address,1));
   modbus_serial_putc(make8(start_address,0));

   modbus_serial_putc(make8(quantity,1));
   modbus_serial_putc(make8(quantity,0));

   modbus_serial_putc(count);

   for(i=0; i < count; ++i) 
      modbus_serial_putc(values[i]);

   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
write_multiple_registers
Input:     int8       address            Slave Address
           int16      start_address      Address to start at
           int16      quantity           Amount of coils to write to
           int16*     values             A pointer to an array holding the data to write
Output:    exception                     0 if no error, else the exception
*/
exception modbus_write_multiple_registers(int8 address, int16 start_address, int16 quantity,
                           int16 *values)
{
   int8 i,count;
   
   count = quantity*2;

   modbus_serial_send_start(address, FUNC_WRITE_MULTIPLE_REGISTERS);

   modbus_serial_putc(make8(start_address,1));
   modbus_serial_putc(make8(start_address,0));

   modbus_serial_putc(make8(quantity,1));
   modbus_serial_putc(make8(quantity,0));
   
   modbus_serial_putc(count);

   for(i=0; i < quantity; ++i)
   {
      modbus_serial_putc(make8(values[i],1));
      modbus_serial_putc(make8(values[i],0));
   }

   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
report_slave_id
Input:     int8       address            Slave Address
Output:    exception                     0 if no error, else the exception
*/
exception modbus_report_slave_id(int8 address)
{
   modbus_serial_send_start(address, FUNC_REPORT_SLAVE_ID);
   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
read_file_record
Input:     int8                address            Slave Address
           int8                byte_count         Number of bytes to read
           read_sub_request*   request            Structure holding record information
Output:    exception                              0 if no error, else the exception
*/
exception modbus_read_file_record(int8 address, int8 byte_count, 
                            modbus_read_sub_request *request)
{
   int8 i;

   modbus_serial_send_start(address, FUNC_READ_FILE_RECORD);

   modbus_serial_putc(byte_count);

   for(i=0; i < (byte_count/7); i+=7)
   {
      modbus_serial_putc(request->reference_type);
      modbus_serial_putc(make8(request->file_number, 1));
      modbus_serial_putc(make8(request->file_number, 0));
      modbus_serial_putc(make8(request->record_number, 1));
      modbus_serial_putc(make8(request->record_number, 0));
      modbus_serial_putc(make8(request->record_length, 1));
      modbus_serial_putc(make8(request->record_length, 0));
      request++;
   }

   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
write_file_record
Input:     int8                address            Slave Address
           int8                byte_count         Number of bytes to read
           read_sub_request*   request            Structure holding record/data information
Output:    exception                              0 if no error, else the exception
*/
exception modbus_write_file_record(int8 address, int8 byte_count, 
                            modbus_write_sub_request *request)
{
   int8 i, j=0;

   modbus_serial_send_start(address, FUNC_WRITE_FILE_RECORD);

   modbus_serial_putc(byte_count);

   for(i=0; i < byte_count; i+=(7+(j*2)))
   {
      modbus_serial_putc(request->reference_type);
      modbus_serial_putc(make8(request->file_number, 1));
      modbus_serial_putc(make8(request->file_number, 0));
      modbus_serial_putc(make8(request->record_number, 1));
      modbus_serial_putc(make8(request->record_number, 0));
      modbus_serial_putc(make8(request->record_length, 1));
      modbus_serial_putc(make8(request->record_length, 0));

      for(j=0; (j < request->record_length) && 
            (j < MODBUS_SERIAL_RX_BUFFER_SIZE-8); j+=2)
      {
         modbus_serial_putc(make8(request->data[j], 1));
         modbus_serial_putc(make8(request->data[j], 0));
      }
      request++;
   }

   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
mask_write_register
Input:     int8       address            Slave Address
           int16      reference_address  Address to mask
           int16      AND_mask           A mask to AND with the data at reference_address
           int16      OR_mask            A mask to OR with the data at reference_address
Output:    exception                              0 if no error, else the exception
*/
exception modbus_mask_write_register(int8 address, int16 reference_address,
                           int16 AND_mask, int16 OR_mask)
{
   modbus_serial_send_start(address, FUNC_MASK_WRITE_REGISTER);

   modbus_serial_putc(make8(reference_address,1));
   modbus_serial_putc(make8(reference_address,0));

   modbus_serial_putc(make8(AND_mask,1));
   modbus_serial_putc(make8(AND_mask,0));

   modbus_serial_putc(make8(OR_mask, 1));
   modbus_serial_putc(make8(OR_mask, 0));

   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
read_write_multiple_registers
Input:     int8       address                Slave Address
           int16      read_start             Address to start reading
           int16      read_quantity          Amount of registers to read
           int16      write_start            Address to start writing
           int16      write_quantity         Amount of registers to write
           int16*     write_registers_value  Pointer to an aray us to write
Output:    exception                         0 if no error, else the exception
*/
exception modbus_read_write_multiple_registers(int8 address, int16 read_start,
                                    int16 read_quantity, int16 write_start,
                                    int16 write_quantity,
                                    int16 *write_registers_value)
{
   int8 i;

   modbus_serial_send_start(address, FUNC_READ_WRITE_MULTIPLE_REGISTERS);

   modbus_serial_putc(make8(read_start,1));
   modbus_serial_putc(make8(read_start,0));

   modbus_serial_putc(make8(read_quantity,1));
   modbus_serial_putc(make8(read_quantity,0));

   modbus_serial_putc(make8(write_start, 1));
   modbus_serial_putc(make8(write_start, 0));

   modbus_serial_putc(make8(write_quantity, 1));
   modbus_serial_putc(make8(write_quantity, 0));

   modbus_serial_putc((int8)(2*write_quantity));

   for(i=0; i < write_quantity ; i+=2)
   {
      modbus_serial_putc(make8(write_registers_value[i], 1));
      modbus_serial_putc(make8(write_registers_value[i+1], 0));
   }

   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

/*
read_FIFO_queue
Input:     int8       address           Slave Address
           int16      FIFO_address      FIFO address
Output:    exception                    0 if no error, else the exception
*/
exception modbus_read_FIFO_queue(int8 address, int16 FIFO_address)
{
   modbus_serial_send_start(address, FUNC_READ_FIFO_QUEUE);

   modbus_serial_putc(make8(FIFO_address, 1));
   modbus_serial_putc(make8(FIFO_address, 0));

   modbus_serial_send_stop();

   MODBUS_SERIAL_WAIT_FOR_RESPONSE();

   return modbus_rx.error;
}

#else
/*MODBUS Slave Functions*/

/********************************************************************
The following structs are used for read/write_sub_request_rsp.  These
functions take in one of these structs.  Please refer to the MODBUS
protocol specification if you do not understand the members of the
structure.
********************************************************************/
typedef struct _modbus_read_sub_request_rsp
{
   int8 record_length;
   int8 reference_type;
   int16 data[((MODBUS_SERIAL_RX_BUFFER_SIZE)/2)-3];
} modbus_read_sub_request_rsp;

typedef struct _modbus_write_sub_request_rsp
{
   int8 reference_type;
   int16 file_number;
   int16 record_number;
   int16 record_length;
   int16 data[((MODBUS_SERIAL_RX_BUFFER_SIZE)/2)-8];
} modbus_write_sub_request_rsp;


/********************************************************************
The following slave functions are defined in the MODBUS protocol.
Please refer to http://www.modbus.org for the purpose of each of
these.  All functions take the slaves address as their first
parameter.
********************************************************************/

/*
read_coils_rsp
Input:     int8       address            Slave Address
           int8       byte_count         Number of bytes being sent
           int8*      coil_data          Pointer to an array of data to send
Output:    void
*/
void modbus_read_coils_rsp(int8 address, int8 byte_count, int8* coil_data)
{
   int8 i;

   modbus_serial_send_start(address, FUNC_READ_COILS);

   modbus_serial_putc(byte_count);

   for(i=0; i < byte_count; ++i)
   {
      modbus_serial_putc(*coil_data);
      coil_data++;
   }

   modbus_serial_send_stop();
}

/*
read_discrete_input_rsp
Input:     int8       address            Slave Address
           int8       byte_count         Number of bytes being sent
           int8*      input_data         Pointer to an array of data to send
Output:    void
*/
void modbus_read_discrete_input_rsp(int8 address, int8 byte_count, 
                                    int8 *input_data)
{
   int8 i;

   modbus_serial_send_start(address, FUNC_READ_DISCRETE_INPUT);

   modbus_serial_putc(byte_count);

   for(i=0; i < byte_count; ++i)
   {
      modbus_serial_putc(*input_data);
      input_data++;
   }

   modbus_serial_send_stop();
}

/*
read_holding_registers_rsp
Input:     int8       address            Slave Address
           int8       byte_count         Number of bytes being sent
           int8*      reg_data           Pointer to an array of data to send
Output:    void
*/
void modbus_read_holding_registers_rsp(int8 address, int8 byte_count, 
                                        int16 *reg_data)
{
   int8 i;

   modbus_serial_send_start(address, FUNC_READ_HOLDING_REGISTERS);

   modbus_serial_putc(byte_count);

   for(i=0; i < byte_count; i+=2)
   {
      modbus_serial_putc(make8(*reg_data,1));
      modbus_serial_putc(make8(*reg_data,0));
      reg_data++;
   }

   modbus_serial_send_stop();
}

/*
read_input_registers_rsp
Input:     int8       address            Slave Address
           int8       byte_count         Number of bytes being sent
           int8*      input_data         Pointer to an array of data to send
Output:    void
*/
void modbus_read_input_registers_rsp(int8 address, int8 byte_count, 
                                        int16 *input_data)
{
   int8 i;

   modbus_serial_send_start(address, FUNC_READ_INPUT_REGISTERS);

   modbus_serial_putc(byte_count);

   for(i=0; i < byte_count; i+=2)
   {
      modbus_serial_putc(make8(*input_data,1));
      modbus_serial_putc(make8(*input_data,0));
      input_data++;
   }

   modbus_serial_send_stop();
}

/*
write_single_coil_rsp
Input:     int8       address            Slave Address
           int16      output_address     Echo of output address received
           int16      output_value       Echo of output value received
Output:    void
*/
void modbus_write_single_coil_rsp(int8 address, int16 output_address, 
                                    int16 output_value)
{
   modbus_serial_send_start(address, FUNC_WRITE_SINGLE_COIL);

   modbus_serial_putc(make8(output_address,1));
   modbus_serial_putc(make8(output_address,0));

   modbus_serial_putc(make8(output_value,1));
   modbus_serial_putc(make8(output_value,0));

   modbus_serial_send_stop();
}

/*
write_single_register_rsp
Input:     int8       address            Slave Address
           int16      reg_address        Echo of register address received
           int16      reg_value          Echo of register value received
Output:    void
*/
void modbus_write_single_register_rsp(int8 address, int16 reg_address, 
                                        int16 reg_value)
{
   modbus_serial_send_start(address, FUNC_WRITE_SINGLE_REGISTER);

   modbus_serial_putc(make8(reg_address,1));
   modbus_serial_putc(make8(reg_address,0));

   modbus_serial_putc(make8(reg_value,1));
   modbus_serial_putc(make8(reg_value,0));

   modbus_serial_send_stop();
}

/*
read_exception_status_rsp
Input:     int8       address            Slave Address
Output:    void
*/
void modbus_read_exception_status_rsp(int8 address, int8 data)
{
   modbus_serial_send_start(address, FUNC_READ_EXCEPTION_STATUS);
   modbus_serial_send_stop();
}

/*
diagnostics_rsp
Input:     int8       address            Slave Address
           int16      sub_func           Echo of sub function received
           int16      data               Echo of data received
Output:    void
*/
void modbus_diagnostics_rsp(int8 address, int16 sub_func, int16 data)
{
   modbus_serial_send_start(address, FUNC_DIAGNOSTICS);

   modbus_serial_putc(make8(sub_func,1));
   modbus_serial_putc(make8(sub_func,0));

   modbus_serial_putc(make8(data,1));
   modbus_serial_putc(make8(data,0));

   modbus_serial_send_stop();
}

/*
get_comm_event_counter_rsp
Input:     int8       address            Slave Address
           int16      status             Status, refer to MODBUS documentation
           int16      event_count        Count of events
Output:    void
*/
void modbus_get_comm_event_counter_rsp(int8 address, int16 status, 
                                        int16 event_count)
{
   modbus_serial_send_start(address, FUNC_GET_COMM_EVENT_COUNTER);

   modbus_serial_putc(make8(status, 1));
   modbus_serial_putc(make8(status, 0));

   modbus_serial_putc(make8(event_count, 1));
   modbus_serial_putc(make8(event_count, 0));

   modbus_serial_send_stop();
}

/*
get_comm_event_counter_rsp
Input:     int8       address            Slave Address
           int16      status             Status, refer to MODBUS documentation
           int16      event_count        Count of events
           int16      message_count      Count of messages
           int8*      events             Pointer to event data
           int8       events_len         Length of event data in bytes
Output:    void
*/
void modbus_get_comm_event_log_rsp(int8 address, int16 status,
                                    int16 event_count, int16 message_count, 
                                    int8 *events, int8 events_len)
{
   int8 i;
    
   modbus_serial_send_start(address, FUNC_GET_COMM_EVENT_LOG);

   modbus_serial_putc(events_len+6);

   modbus_serial_putc(make8(status, 1));
   modbus_serial_putc(make8(status, 0));

   modbus_serial_putc(make8(event_count, 1));
   modbus_serial_putc(make8(event_count, 0));

   modbus_serial_putc(make8(message_count, 1));
   modbus_serial_putc(make8(message_count, 0));

   for(i=0; i < events_len; ++i)
   {
      modbus_serial_putc(*events);
      events++;
   }

   modbus_serial_send_stop();
}

/*
write_multiple_coils_rsp
Input:     int8       address            Slave Address
           int16      start_address      Echo of address to start at
           int16      quantity           Echo of amount of coils written to
Output:    void
*/
void modbus_write_multiple_coils_rsp(int8 address, int16 start_address, 
                                        int16 quantity)
{
   modbus_serial_send_start(address, FUNC_WRITE_MULTIPLE_COILS);

   modbus_serial_putc(make8(start_address,1));
   modbus_serial_putc(make8(start_address,0));

   modbus_serial_putc(make8(quantity,1));
   modbus_serial_putc(make8(quantity,0));

   modbus_serial_send_stop();
}

/*
write_multiple_registers_rsp
Input:     int8       address            Slave Address
           int16      start_address      Echo of address to start at
           int16      quantity           Echo of amount of registers written to
Output:    void
*/
void modbus_write_multiple_registers_rsp(int8 address, int16 start_address, 
                                            int16 quantity)
{
   modbus_serial_send_start(address, FUNC_WRITE_MULTIPLE_REGISTERS);

   modbus_serial_putc(make8(start_address,1));
   modbus_serial_putc(make8(start_address,0));

   modbus_serial_putc(make8(quantity,1));
   modbus_serial_putc(make8(quantity,0));

   modbus_serial_send_stop();
}

/*
report_slave_id_rsp
Input:     int8       address            Slave Address
           int8       slave_id           Slave Address
           int8       run_status         Are we running?
           int8*      data               Pointer to an array holding the data
           int8       data_len           Length of data in bytes
Output:    void
*/
void modbus_report_slave_id_rsp(int8 address, int8 slave_id, int1 run_status,
                              int8 *data, int8 data_len)
{
   int8 i;

   modbus_serial_send_start(address, FUNC_REPORT_SLAVE_ID);

   modbus_serial_putc(data_len+2);
   modbus_serial_putc(slave_id);

   if(run_status)
    modbus_serial_putc(0xFF);
   else
    modbus_serial_putc(0x00);

   for(i=0; i < data_len; ++i)
   {
      modbus_serial_putc(*data);
      data++;
   }

   modbus_serial_send_stop();
}

/*
read_file_record_rsp
Input:     int8                     address            Slave Address
           int8                     byte_count         Number of bytes to send
           read_sub_request_rsp*    request            Structure holding record/data information
Output:    void
*/
void modbus_read_file_record_rsp(int8 address, int8 byte_count, 
                                    modbus_read_sub_request_rsp *request)
{
   int8 i=0,j;

   modbus_serial_send_start(address, FUNC_READ_FILE_RECORD);

   modbus_serial_putc(byte_count);

   while(i < byte_count);
   {
      modbus_serial_putc(request->record_length);
      modbus_serial_putc(request->reference_type);

      for(j=0; (j < request->record_length); j+=2)
      {
         modbus_serial_putc(make8(request->data[j], 1));
         modbus_serial_putc(make8(request->data[j], 0));
      }

      i += (request->record_length)+1;
      request++;
   }

   modbus_serial_send_stop();
}

/*
write_file_record_rsp
Input:     int8                     address            Slave Address
           int8                     byte_count         Echo of number of bytes sent
           write_sub_request_rsp*   request            Echo of Structure holding record information
Output:    void
*/
void modbus_write_file_record_rsp(int8 address, int8 byte_count, 
                                    modbus_write_sub_request_rsp *request)
{
   int8 i, j=0;

   modbus_serial_send_start(address, FUNC_WRITE_FILE_RECORD);

   modbus_serial_putc(byte_count);

   for(i=0; i < byte_count; i+=(7+(j*2)))
   {
      modbus_serial_putc(request->reference_type);
      modbus_serial_putc(make8(request->file_number, 1));
      modbus_serial_putc(make8(request->file_number, 0));
      modbus_serial_putc(make8(request->record_number, 1));
      modbus_serial_putc(make8(request->record_number, 0));
      modbus_serial_putc(make8(request->record_length, 1));
      modbus_serial_putc(make8(request->record_length, 0));

      for(j=0; (j < request->record_length); j+=2)
      {
         modbus_serial_putc(make8(request->data[j], 1));
         modbus_serial_putc(make8(request->data[j], 0));
      }
   }

   modbus_serial_send_stop();
}

/*
mask_write_register_rsp
Input:     int8        address            Slave Address
           int16       reference_address  Echo of reference address
           int16       AND_mask           Echo of AND mask
           int16       OR_mask            Echo or OR mask
Output:    void
*/
void modbus_mask_write_register_rsp(int8 address, int16 reference_address,
                           int16 AND_mask, int16 OR_mask)
{
   modbus_serial_send_start(address, FUNC_MASK_WRITE_REGISTER);

   modbus_serial_putc(make8(reference_address,1));
   modbus_serial_putc(make8(reference_address,0));

   modbus_serial_putc(make8(AND_mask,1));
   modbus_serial_putc(make8(AND_mask,0));

   modbus_serial_putc(make8(OR_mask, 1));
   modbus_serial_putc(make8(OR_mask, 0));

   modbus_serial_send_stop();
}

/*
read_write_multiple_registers_rsp
Input:     int8        address            Slave Address
           int16*      data               Pointer to an array of data
           int8        data_len           Length of data in bytes
Output:    void
*/
void modbus_read_write_multiple_registers_rsp(int8 address, int8 data_len, 
                                                int16 *data)
{
   int8 i;

   modbus_serial_send_start(address, FUNC_READ_WRITE_MULTIPLE_REGISTERS);

   modbus_serial_putc(data_len*2);

   for(i=0; i < data_len*2; i+=2)
   {
      modbus_serial_putc(make8(data[i], 1));
      modbus_serial_putc(make8(data[i], 0));
   }

   modbus_serial_send_stop();
}

/*
read_FIFO_queue_rsp
Input:     int8        address            Slave Address
           int16       FIFO_len           Length of FIFO in bytes
           int16*      data               Pointer to an array of data
Output:    void
*/
void modbus_read_FIFO_queue_rsp(int8 address, int16 FIFO_len, int16 *data)
{
   int8 i;
   int16 byte_count;

   byte_count = ((FIFO_len*2)+2);

   modbus_serial_send_start(address, FUNC_READ_FIFO_QUEUE);

   modbus_serial_putc(make8(byte_count, 1));
   modbus_serial_putc(make8(byte_count, 0));

   modbus_serial_putc(make8(FIFO_len, 1));
   modbus_serial_putc(make8(FIFO_len, 0));

   for(i=0; i < FIFO_len; i+=2)
   {
      modbus_serial_putc(make8(data[i], 1));
      modbus_serial_putc(make8(data[i], 0));
   }

   modbus_serial_send_stop();
}

void modbus_exception_rsp(int8 address, int16 func, exception error)
{
   modbus_serial_send_start(address, func|0x80);
   modbus_serial_putc(error);
   modbus_serial_send_stop();
}

#endif
