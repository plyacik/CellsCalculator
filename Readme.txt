Spreadsheet processor. It must be able to process cells just like in a ordinary spreadsheet, but will use simplified expressions. 
Each cell may contain:
- nothing;
- integer or float number;
- text labels, which is started with ' symbol;
- expression, which is started with '=' symbol and may contain numbers, cell references, and simple arithmetic operations ('+', '-', '*', '/', '^') 
with cell references. The priority of operations can be changed with the characters "(" and ")".

Example of Data input:
12		=C2	3	'Sample
=(A1+B1)*C1/5	=A2*B1	=B3-C3	'Spread
'Test		=4-3	5	'Sheet

Example of Data output:
12	-4	3	Sample
4,8	-19,2	-4	Spread
Test	1	5	Sheet
