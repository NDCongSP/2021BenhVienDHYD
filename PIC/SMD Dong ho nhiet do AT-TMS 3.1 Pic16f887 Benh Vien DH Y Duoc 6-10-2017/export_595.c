void port_init(int8 *my_array_port)
{
   //----------------------Cum dieu khien 595 thu nhat------------------------------ 
   output_low(my_array_port[0]); //DATA 
   output_low(my_array_port[1]); //SCK
   output_low(my_array_port[2]); //RCK
}
//Module ham dieu khien thanh ghi dich

void load_shift_reg(int8 *shift_reg_control_array);
void load_store_reg(int8 *shift_reg_control_array);
#separate
void send_byte(int8 data,int8 * shift_reg_control_array)
{

 int8 z; 

 for(z = 0; z < 8; z++)                      // Send 8 bits 
    {    
     if(data & 0x80)                         //kiem tra xem bit thu 8 la 0 hay 1
       {  //neu la 1    
        output_high(shift_reg_control_array[0]);              //cho pin DATA len muc cao 
        delay_us(10);
       } 
     else //neu la 0
       { 
        output_low(shift_reg_control_array[0]);               //cho pin DATA xuong muc thap
        delay_us(10);
       } 


       //dich du lieu
       output_high(shift_reg_control_array[1]); 
       delay_us(30);
       output_low(shift_reg_control_array[1]);                 // tao 1 xung clock L to H cho SCK.
     data <<= 1;                                               // Dich sang trai 1 bit va gan vao data      
   }    

}
//***********************************************************************************************************************************************
     
//****************************************Ham cho phep dua du lieu ra cac chan 595***************************************************************
void load_shift_reg(int8 *shift_reg_control_array) 
{ 
   output_high(shift_reg_control_array[1]); 
   delay_us(30);
   output_low(shift_reg_control_array[1]);                 // tao 1 xung clock L to H cho SCK.
   
}
void load_store_reg(int8 *shift_reg_control_array) 
{ 
   output_high(shift_reg_control_array[2]);                 // tao 1 xung clock H to L cho RCK 
   delay_ms(20);
   output_low(shift_reg_control_array[2]); 
} 


