package de.popforge.asm
{
	/**
	 * The __asm function is a prototype that allows you to write inline assembler
	 * instructions by using ActionScript 3 syntax.
	 * 
	 * <p>The As3c compiler will find the __asm method in your code and replace all
	 * the instructions with the corresponding bytecode.</p>  
  	 *
	 * <p><b>Note:</b> You may use constant parameters only. Syntax like __asm(Op.PushInt(i++))
	 * is not allowed and will result in an error message by the compiler.</p>
	 * 
	 * @author Joa Ebert
	 * @see Op
	 * 
	 * @example
	 * <pre>
	 * __asm(
	 * 	Op.findPropertyStrict('public::trace'),
	 * 	Op.pushString('Hello World!'),
	 *	Op.callPropertyVoid('public::trace', 1)
	 * );
	 * </pre>
	 */	
	public function __asm(... instructions):void{}
}