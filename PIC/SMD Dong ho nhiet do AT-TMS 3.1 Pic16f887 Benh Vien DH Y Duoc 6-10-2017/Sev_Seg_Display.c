#include <export_595.c>

void display(int8 *mydisplay_data, int8 mysev_num, int8 *myport);
                               // 0,        1,          2,        3,         4,         5,        6,          7,         8,          9,    -,  "",     .,           E,         r,         t,          I,          H,          L,          d,           b,         C          O           u        A
int8 const sev_seg_map[25]={0b00101000,0b01111110,0b00110001,0b00110100,0b01100110,0b10100100,0b10100000,0b00111110,0b00100000,0b00100100,0xF7,0xFF,0b00100000,0b10100001,0b11110011, 0b11100001, 0b11111110, 0b01100010, 0b11101001, 0b01110000, 0b11100000,0b10101001,0b00101000,0b11111000,0b00100010};

void display(int8 *mydisplay_data, int8 mysev_num, int8 *myport)
{
   int8 display_buffer[MAXNUM];
   int8 i, j;

   for(i=mysev_num-1, j=0; j<mysev_num; i--, j++)
   {  
      display_buffer[j]=sev_seg_map[mydisplay_data[i]];  
   }
   if(Daucham==1)
   {
      display_buffer[1]=display_buffer[1]^sev_seg_map[12]; 
   }   
   if(Led_Alarm==1)
   {
      display_buffer[0]=display_buffer[0]^sev_seg_map[12]; 
   }
   //do 3 times for ensuring
   for(j=0; j<1; j++)
   {
      // send byte                 
      for (i=0;i<mysev_num;i++)
      {
         send_byte(display_buffer[i],myport);
      }
      load_store_reg(myport);
      delay_ms(1);
   }  
}


                   
