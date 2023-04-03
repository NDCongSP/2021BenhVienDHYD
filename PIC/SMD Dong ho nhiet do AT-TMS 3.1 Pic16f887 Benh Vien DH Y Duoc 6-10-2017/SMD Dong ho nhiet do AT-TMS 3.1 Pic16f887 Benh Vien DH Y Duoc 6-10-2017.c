#include <16f887.h>
#fuses WDT, HS, NOPUT,PROTECT, NODEBUG, NOBROWNOUT, NOLVP, NOCPD
#use delay (clock=20000000)
#use fast_io(a)
#use fast_io(b)
#use fast_io(d)
const int8 MAXNUM=4;//num of sev_segs
int8 i=0,m=0;
int1 daucham=0,Modbus_update=0,Ghi_Rom=0;
int8 control_array[3]={Pin_A0,Pin_A1,Pin_A2};
int1 Old_Mode=0,Old_Alarm=0,alarmACK=0;
int8 canhbao=0,set_count=0,Config=0;
float Temp_offset=0,deadband=0;
signed int8 high_temp=0,low_temp=0;
int8 nhapnhay=0,Delay_ReadTemp=0;
int1 Count_ReadTemp=0;
//PACK FOR NEW CLOCK
int8 main_array[MAXNUM]={0,0,0,0};
int8 temp_array[MAXNUM]={0,0,0,0};
int1 alarmbit[2]={0,0},Led_Alarm=0;
#include <modbus_slave.c>
#include <Ds18d20.c>
#include <Sev_Seg_Display.c>
//=======================//
/////////////////////////////////////////////////////////////////////////////////
Void Doc_eeprom();
Void Alarm_Control();
Void Nutnhan();
Void CaiDat();
Void Calib_temp();
Void Ghi_eeprom();
void Apply_Changes();
void main()
{  
   enable_interrupts(GLOBAL);
   setup_adc_ports( NO_ANALOGS);
   set_tris_a(0);
   set_tris_b(0);
   set_tris_c(0);
   set_tris_d(0b11110000);
   set_tris_e(0);
   
   output_a(0);
   output_b(0);
   output_c(0);
   output_d(0);
   output_e(0);   
   port_init(control_array);  
   Setup_WDT(WDT_2304MS);
   Get_temp();
   daucham=0;
   main_array[0] = 11;
   main_array[1] = 11;
   main_array[2] = 16;
   main_array[3] = 19;
   display(main_array, MAXNUM, control_array);  
   delay_ms(1000);
   main_array[0] = 11;
   main_array[1] = 11;
   main_array[2] = MODBUS_ADDRESS/10;
   main_array[3] = MODBUS_ADDRESS%10;   
   display(main_array, MAXNUM, control_array);     
   delay_ms(1000);
   Get_temp();
   // Modbus init
   modbus_init();      
   big_hold_regs[6]=0;
   Doc_eeprom();
   While(true)
   {                     
      restart_wdt();
      if(Count_ReadTemp==0)
      {
         if(modbus_kbhit())
         {          
            modbus_process();
            if(Config==0)
            {
               Modbus_update=1;
            }
         }  
         if(Modbus_update==1)
         {
            Apply_Changes(); 
            Modbus_update=0;      
         }
         delay_ms(100);
         Count_ReadTemp=1;
      }
      if(Config==0)
      {       
         if(Count_ReadTemp==1)
         {
            disable_interrupts(GLOBAL);
            Get_temp();
            Delay_ms(100);
            Count_ReadTemp=0;
            enable_interrupts(GLOBAL);
         }
         display(main_array, MAXNUM, control_array);
         Alarm_Control();
         Nutnhan();
         if(Ghi_Rom==1)
         {
            Ghi_eeprom();
            Ghi_Rom=0;
         }
      }
      else if(Config==1)
      {                   
         CaiDat(); 
      }
      else if ( config>1)      
      {
         Config=0;
      }
   }
}
Void Doc_eeprom()
{
   //Read EEProm for offset values
   // High Alarm
   for(i=0;i<3;i++)
   {
      main_array[i]=read_eeprom(i);            
      //anti noise
      if(i==0)
      {
         continue;
      }
      
      if(main_array[i]>9)
      {
         main_array[i]=0;//if there is any noise - > change to "0"
         write_eeprom(i,main_array[i]);
      }
   }               
   if(main_array[0] == 10)
   {
      high_temp = ((float)(main_array[1]*10+main_array[2]));
      high_temp = high_temp*(-1);
   }
   else
   {
      high_temp = (float)(main_array[0]*100 + main_array[1]*10 + main_array[2]);
   }
   big_hold_regs[1]=high_temp;
   // Low Alarm
   for(i=0;i<3;i++)
   {
      main_array[i]=read_eeprom(i+3);            
      //anti noise
      if(i==0)
      {
         continue;
      }      
      if(main_array[i]>9)
      {
         main_array[i]=0;//if there is any noise - > change to "0"
         write_eeprom(i+3,main_array[i]);
      }
   }               
   if(main_array[0] == 10)
   {
      low_temp = ((float)(main_array[1]*10+main_array[2]));
      low_temp = low_temp*(-1);
   }
   else
   {
      low_temp = (float)(main_array[0]*100 + main_array[1]*10 + main_array[2]);
   }
   big_hold_regs[2]=low_temp;
   // Deadband
   Deadband = read_eeprom(6);
   big_hold_regs[3]=Deadband;
   // Calib
   for(i=0;i<3;i++)
   {
      main_array[i]=read_eeprom(i+7);            
      //anti noise
      if(i==0)
      {
         continue;
      }
      
      if(main_array[i]>9)
      {
         main_array[i]=0;//if there is any noise - > change to "0"
         write_eeprom(i+7,main_array[i]);
      }
   }               
   temp_offset = ((float)(main_array[1]*10+main_array[2]));
   if(main_array[0] == 10)
   {
      temp_offset = temp_offset*(-1);
   }
   big_hold_regs[4]=Temp_offset;
}
Void Alarm_Control()
{
      // Bao chuong canh bao            
      if(temp  > ( high_temp+ deadband/10)) 
         alarmbit[0]=1;             //canh bao muc cao  cua nhiet do
      else if(temp < (high_temp- deadband/10)) 
         alarmbit[0]=0;
      if(temp < (low_temp - deadband/10))
         alarmbit[1]=1;            //// canh bao muc thap cua nhiet do      
      else if(temp > (low_temp + deadband/10))
         alarmbit[1]=0;
      
      Canhbao=alarmbit[0]+alarmbit[1];
      //Alarm ACK
      if((input(pin_D5)==0)&&(input(pin_D6)==1)&&(Old_Alarm==0))
      {
         delay_ms(5);
         if((input(pin_D5)==0)&&(input(pin_D6)==1)&&(Old_Alarm==0))
         {
            Old_Alarm=1;
         }
      }
      else if((input(pin_D5)==1)&&(input(pin_D6)==1)&&(Old_Alarm==1))
      {
         alarmACK=1;
         Old_Alarm=0;
      }
   //Control horn temp
   if(Read_ok==1)
   {
      if((canhbao>0) && (alarmACK==0)&&(read_ok==1))
      {
         output_high(pin_D0);
         if(nhapnhay >= 1)
         {
            nhapnhay=0;                  
         }
         else
         {
            nhapnhay++;
         }
         
         if(nhapnhay >= 1)
         {
            Led_Alarm=1;
         }
         else
         {
            Led_Alarm=0;
         }
      }
      else if((canhbao>0) && (alarmACK==1)&&(read_ok==1))
      {
         output_low(pin_D0);
         if(nhapnhay >= 10)
         {
            nhapnhay=0;                  
         }
         else
         {
            nhapnhay++;
            if(nhapnhay >= 5)
            {
               Led_Alarm=1;
            }
            else
            {
               Led_Alarm=0;
            }
         }
      }
      else if(canhbao==0)
      {
         alarmACK=0;
         Led_Alarm=0;
         output_low(pin_D0);
      } 
   }
   else
   {
      alarmACK=0;
      Led_Alarm=0;
      output_high(pin_D0);   
   }
}
Void Nutnhan()
{
   //Cofig chinh nhiet do bao chuong
   if((input(pin_D7)==0)&&(Old_Mode==0))
   {
      delay_ms(10);
      if((input(pin_D7)==0)&&(Old_Mode==0))
      {            
         Old_Mode=1;
      }
   }
   else if((input(pin_D7)==1)&&(Old_Mode==1))
   {
      Config=1;
      Set_count=1;
      Old_Mode=0;
   } 
}
Void CaiDat()
{
   Led_Alarm=0;
   Modbus_update=0;
   Canhbao=0;
   alarmbit[0]=0;
   alarmbit[1]=0;
   alarmACK=0;
   Count_ReadTemp=0;
   output_low(pin_D0);   
   if(set_count ==1)
   {
      Daucham=0;
      if(high_temp>=0)
      {
         temp_array[0]= 17;//H
         temp_array[1]= (int8)high_temp/100 ;
         temp = (int8)high_temp%100;
         temp_array[2]= (int8)temp/10;
         temp_array[3]= (int8)temp%10;  
      }
      else
      {
         temp_array[0]= 17;//H
         temp_array[1]= 10;
         temp_array[2]= (int8)(high_temp*(-1))/10 ;              
         temp_array[3]= (int8)(high_temp*(-1))%10 ;              
      }
   } 
   else if(set_count ==2)
   {
      Daucham=0;
      if(low_temp>=0)
      {
         temp_array[0]= 18;//L
         temp_array[1]= (int8)low_temp/100 ;
         temp = (int8)low_temp%100;
         temp_array[2]= (int8)temp/10;
         temp_array[3]= (int8)temp%10;  
      }
      else
      {
         temp_array[0]= 18;//L
         temp_array[1]= 10;
         temp_array[2]= (int8)(low_temp*(-1))/10 ;              
         temp_array[3]= (int8)(low_temp*(-1))%10 ;              
      }
   }
   else if(set_count ==3)
   {
      Daucham=1;
      temp_array[0]= 19;//b
      temp_array[1]= 20;//b
      temp_array[2]= (int8)deadband/10;
      temp_array[3]= (int8)deadband%10;      
   }         
   else if(set_count ==4)
   {
      Daucham=1;
      if(temp_offset < 0)
      {
         temp_array[0]=21;//C
         temp_array[1]=10;
         temp_array[2]=(int8)(temp_offset*(-1))/10;
         temp_array[3]=(int8)(temp_offset*(-1))%10;         
      }
      else
      {
         temp_array[0]=21;//C
         temp_array[1]=11;
         temp_array[2]=(int8)temp_offset/10;
         temp_array[3]=(int8)temp_offset%10;
      }
      display(temp_array,MAXNUM, control_array);// hien thi ra led 7 doan 
   }      

   //Cong
   if(input(pin_D6)==0)
   {
      delay_ms(50);
      if(input(pin_D6)==0)
      {
         delay_ms(50);
         if(set_count==1)
         {
            if(high_temp >= 125)
            {
               high_temp =125;
            }
            else
            {
               high_temp = high_temp + 1;
            }
         }
         else if(set_count==2)
         {
            if(low_temp >= 125)
            {
               low_temp =125;
            }
            else
            {
               low_temp = low_temp + 1;
            }
         }
         else if(set_count==3)
         {
            if(deadband >= 50)
            {
               deadband =50;
            }
            else
            {
               deadband = deadband + 1;
            }
         }
         else if(set_count==4)
         {
            if(temp_offset >= 99)
            {
               temp_offset =99;
            }
            else
            {
               temp_offset = temp_offset + 1;
            }
         }         
      }
   }
   //Tru
   else if(input(pin_D5)==0)
   {
      delay_ms(50);
      if(input(pin_D5)==0)
      {
         delay_ms(50);
         if(set_count==1)
         {
            if(high_temp <= -55)
            {
               high_temp =-55;
            }
            else
            {
               high_temp = high_temp - 1;
            }
         }
         else if(set_count==2)
         {
            if(low_temp <= -55)
            {
               low_temp =-55;
            }
            else
            {
               low_temp = low_temp - 1;
            }
         }
         else if(set_count==3)
         {
            if(deadband <= 0)
            {
               deadband =0;
            }
            else
            {
               deadband = deadband - 1;
            }
         }
         else if(set_count==4)
         {
            if(temp_offset <= -99)
            {
               temp_offset =-99;
            }
            else
            {
               temp_offset = temp_offset - 1;
            }
         }               
      }
   }
   //SET
   if(((input(pin_D7)==0)&&(Old_Mode==0)))
   {
      delay_ms(10);
      Old_Mode=1;
   }
   else if(((input(pin_D7)==1)&&(Old_Mode==1)))
   {
      Old_Mode=0;
      set_count++;
      //Return to runtime mode
      if(set_count > 4)
      {
         set_count=1;
         alarmACK=0;
         Config=0;
         big_hold_regs[1] = high_temp; 
         big_hold_regs[2] = low_temp;
         big_hold_regs[3] = deadband; 
         big_hold_regs[4] = Temp_offset; 
         Ghi_rom=1;
      }                  
   }
   display(temp_array,MAXNUM, control_array);// hien thi ra led 7 doan  
}
Void Ghi_eeprom()
{
   // high Alarm
   if(high_temp>=0)
   {
      // Ghi rom
      m=(int8)high_temp/100;
      write_eeprom(0,m);
      m=(int8)(high_temp/10)%10;
      write_eeprom(1,m);
      m=(int8)high_temp%10;
      write_eeprom(2,m);
   }
   else
   {
      // Ghi rom
      m=10;
      write_eeprom(0,m);
      m=(int8)(high_temp*(-1))/10;
      write_eeprom(1,m);
      m=(int8)(high_temp*(-1))%10;
      write_eeprom(2,m);            
   }
   // Low Alarm
   if(low_temp>=0)
   {
      // Ghi rom
      m=(int8)low_temp/100;
      write_eeprom(3,m);
      m=(int8)(low_temp/10)%10;
      write_eeprom(4,m);
      m=(int8)low_temp%10;
      write_eeprom(5,m);
   }
   else
   {
      // Ghi rom
      m=10;
      write_eeprom(3,m);
      m=(int8)(low_temp*(-1))/10;
      write_eeprom(4,m);
      m=(int8)(low_temp*(-1))%10;
      write_eeprom(5,m);            
   } 
   // Deadband
   write_eeprom(6,(int8)Deadband); 
    // Offset
   if(Temp_offset>=0)
   {
      // Ghi rom
      m=0;
      write_eeprom(7,m);
      m=(int8)Temp_offset/10;
      write_eeprom(8,m);
      m=(int8)Temp_offset%10;
      write_eeprom(9,m);            
   }
   else
   {
      // Ghi rom
      m=10;
      write_eeprom(7,m);
      m=(int8)(Temp_offset*(-1))/10;
      write_eeprom(8,m);
      m=(int8)(Temp_offset*(-1))%10;
      write_eeprom(9,m);            
   }
   // Relay Out
   //write_eeprom(10,Relay_out);      
}
void Apply_Changes()
{
   if(big_hold_regs[6]==100)
   {
      // High Temp Alarm
      if(big_hold_regs[1]>125)
      {
         big_hold_regs[1]=125;
      }
      else if(big_hold_regs[1]<(-55))
      {
         big_hold_regs[1]=-55;
      }
      high_temp = big_hold_regs[1];         
      // Low Temp Alarm    
      if(big_hold_regs[2]>125)
      {
         big_hold_regs[2]=125;
      }
      else if(big_hold_regs[2]<(-55))
      {
         big_hold_regs[2]=-55;
      }
      low_temp = big_hold_regs[2];      
      // Deadband     
      if(big_hold_regs[3]>50)
         big_hold_regs[3]=50;
      Deadband = big_hold_regs[3];
      // Temp Calib
      if((big_hold_regs[4]>99)||(big_hold_regs[4]<(-99)))
         big_hold_regs[4]=0;
      Temp_offset=(float)big_hold_regs[4];                  
      
      Ghi_Rom=1;
      big_hold_regs[6]=0;
   }
   else
   {
      big_hold_regs[6]=0;
   }
}
