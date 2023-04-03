//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                         CAC DINH NGHIA VA THU VIEN CHO MODBUS                                                 //
//                                                                                                                               //
   #define MODBUS_ADDRESS 50//Add of MobusSlave, range: 0-->247
   #define MODBUS_TYPE MODBUS_TYPE_SLAVE
   #define MODBUS_SERIAL_TYPE MODBUS_RTU     //use MODBUS_ASCII for ASCII mode
   #define MODBUS_SERIAL_RX_BUFFER_SIZE 64
   #define MODBUS_SERIAL_BAUD 9600
   //Nguon ngat PC to MCU (Pic)
   #define MODBUS_SERIAL_INT_SOURCE MODBUS_INT_RDA
   ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   //                                              KHAI BAO PIN OUT                                                                 //
   //                                                                                                                               //
   #define MODBUS_SERIAL_ENABLE_PIN   PIN_C5   // Controls DE pin for RS485
   #define MODBUS_SERIAL_RX_ENABLE    PIN_C5   // Controls RE pin for RS485                                                         
                                                                                                                                 //
   //                                                                                                                               //
   ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

   #include <MODBUS_LIBRARY.c> //Thu vien chuan cua CCS-C     
//                                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                            KHAI BAO BIEN TOAN CUC                                                             //
   signed int16 big_hold_regs[7];
   int16 big_input_regs[17];   
   int8 regis_num = 7;
   
   int32 big_coils = 0x00000000;
   int32 coil_data_reciev = 0x00000000;
   int32 big_inputs = 0x0000006;//Chua code
   
//#separate
void modbus_process()
{
   //while(!modbus_kbhit());
   
   delay_us(50);
   
   //check address against our address, 0 is broadcast
   if((modbus_rx.address == MODBUS_ADDRESS))// || modbus_rx.address == 0)
   {
      switch(modbus_rx.func)
      {
         case FUNC_READ_HOLDING_REGISTERS:
         case FUNC_READ_INPUT_REGISTERS:
            if(modbus_rx.data[0] || modbus_rx.data[2] ||
               modbus_rx.data[1] >= regis_num || modbus_rx.data[3]+modbus_rx.data[1] > regis_num)     //regis_num: so thanh ghi Hold (Input) khai bao ben tren
               modbus_exception_rsp(MODBUS_ADDRESS,modbus_rx.func,ILLEGAL_DATA_ADDRESS);
            else
            {
               if(modbus_rx.func == FUNC_READ_HOLDING_REGISTERS)
                  modbus_read_holding_registers_rsp(MODBUS_ADDRESS,(modbus_rx.data[3]*2),big_hold_regs+modbus_rx.data[1]);
               else
                  modbus_read_input_registers_rsp(MODBUS_ADDRESS,(modbus_rx.data[3]*2),big_input_regs+modbus_rx.data[1]);
               
            }
            break;
         /*case FUNC_WRITE_SINGLE_REGISTER:
            if(modbus_rx.data[0] || modbus_rx.data[1] >= regis_num)
               modbus_exception_rsp(MODBUS_ADDRESS,modbus_rx.func,ILLEGAL_DATA_ADDRESS);
            else
            {
               //the registers are stored in little endian format
               big_hold_regs[modbus_rx.data[1]] = make16(modbus_rx.data[2],modbus_rx.data[3]);//lay 2 byte du lieu ghi vao thanh ghi "hold_regs"
               
               //Response lai khung du lieu vua nhan tu PC (Du lieu request)
               modbus_write_single_register_rsp(MODBUS_ADDRESS,
                            make16(modbus_rx.data[0],modbus_rx.data[1]),
                            make16(modbus_rx.data[2],modbus_rx.data[3]));
            }
            break;*/
         case FUNC_WRITE_MULTIPLE_REGISTERS:
            if(modbus_rx.data[0] || modbus_rx.data[2] ||
               modbus_rx.data[1] >= regis_num || modbus_rx.data[3]+modbus_rx.data[1] > regis_num)
               modbus_exception_rsp(MODBUS_ADDRESS,modbus_rx.func,ILLEGAL_DATA_ADDRESS);
            else
            {
               int i,j;

               for(i=0,j=5; i < modbus_rx.data[4]/2; ++i,j+=2)
                  big_hold_regs[(i+modbus_rx.data[1])] = make16(modbus_rx.data[j],modbus_rx.data[j+1]);

               modbus_write_multiple_registers_rsp(MODBUS_ADDRESS,
                              make16(modbus_rx.data[0],modbus_rx.data[1]),
                              make16(modbus_rx.data[2],modbus_rx.data[3]));            
            }
            break;           
         default:    //We don't support the function, so return exception
            modbus_exception_rsp(MODBUS_ADDRESS,modbus_rx.func,ILLEGAL_FUNCTION);
      }
   }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
