
#define DQ pin_E0
//----------------------------------------------------------------
#separate
init_1wire(){
   int a;

   output_low(DQ);
   delay_us(480);
   output_float(DQ);
   delay_us(72);
   a=!input(DQ);
   delay_us(425);
   if(a)
      return(1);
   else
      return(0);
}

//-------------------------read byte------------------------------
#separate
byte read_1wire(){
   byte a,data;
   for(a=0;a<8;a++){
      output_low(DQ);
      delay_us(1);
      output_float(DQ);
      delay_us(15);
      shift_right(&data,1,input(DQ));
      delay_us(120);
   }
   return(data);
}   
//--------------------------write byte----------------------------
#separate
byte write_1wire(byte data)
{
   byte a;

   for(a=0;a<8;a++)
   {
      output_low(DQ);
      delay_us(1);
      if(shift_right(&data,1,0))
         output_high(DQ);
      else
         output_low(DQ);
         
      delay_us(104);
      output_high(DQ);
   }
   delay_us(104);
   return(1);
}

byte buffer[8];
int1 read_ok=0;
int1 minus=0;
float temp=0;//,Old_temp=0;
#separate   
void Get_temp()
{      
   if(init_1wire())
   {
      write_1wire(0xcc);                                       //skip ROM
      write_1wire(0x44);                                       //convert T
   }

   if(init_1wire())
   {
      write_1wire(0xcc);                                       //skip ROM
      write_1wire(0xbe);                                       //read scratchpad
      for(i=0;i<8;i++)
      {
         buffer[i]=read_1wire();
         delay_us(10);
      }
      read_ok=1;
   }
   else
   {
      read_ok=0;
   }
   
   if(read_ok)
   {      
      if(buffer[1]>=16)//neu nhiet do la am dao lai buffer[1],buffer[0]
      {
          buffer[1]=~buffer[1];
          buffer[0]=~buffer[0];
          buffer[0]=buffer[0]+1;//hieu chinh lai de hien thi cho dung
          temp=make16(buffer[1],buffer[0]);
          temp= (float) temp/16.0; 
          temp=temp*(-1);
      }
      else
      {
          temp=make16(buffer[1],buffer[0]);
          temp= (float) temp/16.0;
      }
      // Ham lay gia tri nhiet do sau hieu chinh
      temp=temp + 0.6 +(temp_offset/10);
      /*if((( temp - Old_temp)>=3)||(( Old_temp - Temp)>=3))
      {
         temp =Old_temp;
      }
      else
      {
         Old_temp=temp;
      }*/
      if(temp>=0)
      {
         //t = temp;
         minus=0;
      }
      else
      {
         //t = temp*(-1);
         minus=1;
      }
      //neu nhiet do Duong
      if(minus==0)
      {
         if((temp>=0.1)&&(temp<10))
         {
            main_array[0]=11;
            main_array[1]=11;
            main_array[2]=((int8)(temp)%10);
            main_array[3]=((int16)(temp*10)%10);                                    
            Daucham=1;
            big_hold_regs[0]=temp*10;
         }
         else if((temp>=10)&&(temp<100))
         {
            main_array[0]=11;
            main_array[1]=(int8)(temp/10);
            main_array[2]=((int8)(temp)%10);
            main_array[3]=((int16)(temp*10)%10);                                    
            Daucham=1;
            big_hold_regs[0]=temp*10;
         }
         else if((temp>=100)&&(temp<=125))
         {
            main_array[0]=(int8)(temp/100);
            main_array[1]=(int8)(temp/10)%10;
            main_array[2]=(int8)(temp)%10;
            main_array[3]=(int16)(temp*10)%10;
            Daucham=1;
            big_hold_regs[0]=temp*10;
         }         
         else if (temp>125)
         {
            main_array[0]=13;
            main_array[1]=14;
            main_array[2]=14;
            main_array[3]=17;
            Daucham=1;
            big_hold_regs[0]=1260;
         }
         else if(temp<0.1)
         {
            main_array[0]=11;
            main_array[1]=11;
            main_array[2]=11;
            main_array[3]=0;
            Daucham=0;
            big_hold_regs[0]=0;
         }
      }      
      else//neu nhiet do Am
      {
         if((temp>(-10))&& (temp<=(-0.1)))
         {
            main_array[3]=((int16)(temp*(-10))%10);
            main_array[2]=(int16)(temp*(-1))%10;
            main_array[1]=10;
            main_array[0]=11;
            Daucham=1;
            big_hold_regs[0]=temp*10;
         }
         else if((temp<=(-10))&& (temp>=(-55)))
         {
            main_array[3]=((int16)(temp*(-10))%10);
            main_array[2]=(int16)(temp*(-1))%10;
            main_array[1]=(int8)(temp*(-1))/10;
            main_array[0]=10;
            Daucham=1;
            big_hold_regs[0]=temp*10;
         }         
         else if(temp<(-55))
         {
            main_array[0]=13;
            main_array[1]=14;
            main_array[2]=14;
            main_array[3]=18;
            Daucham=1;
            big_hold_regs[0]=-56;
         }
       }     
   }
   else
   {
      main_array[0]=13;
      main_array[1]=14;
      main_array[2]=14;
      main_array[3]=5;
      Daucham=1;      
      big_hold_regs[0]=9998;
   }
}


