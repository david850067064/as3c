package de.popforge.asm
{
	/**
	 * Marks a function to be inlined. If you inline a function the actual code
	 * of the function will be inserted right at the place where you put if.
	 *
	 * @author Joa Ebert
	 * @example
	 * Here we have the definition and use of two functions:
	 * <pre>
	 * private function doSomething(x: Number): void
	 * {
	 * 	__inline();
	 * 	return x*x;
	 * }
	 * 
	 * private function callSomething(x: Number): void
	 * {
	 * 	return doSomething(x);
	 * }
	 * </pre>
	 * 
	 * The example is of course very stupid but you will get the point what
	 * you can do very quickly. This is what it would compile to using As3c.
	 * <pre>
	 * private function callSomething(x: Number): void
	 * {
	 * 	return x*x;
	 * }
	 * </pre>
	 * 
	 * The call to doSomething() has been replaced with the actual logic that
	 * happens inside doSomething() because we marked it as inline.
	 * 
	 * This is a very powerful feature because you will get rid of the function
	 * call which takes a big amount of time.
	 * 
	 * @author Joa Ebert
	 */
	public function __inline():void{}
}