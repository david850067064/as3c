# Deprecated #
## As3c is no longer in development. Checkout [Apparat](http://apparat.googlecode.com/) instead. It includes a more advanced and robust inline assembler plus tons of other features. ##


---


# As3c #
As3c is a bytecode compiler for the AVM2 written in C#.

## Features ##
It allows you to...
  * compile inline assemlber instructions written in in ActionScript 3 while maintining full debugging capabilities.
  * compile pure bytecode into a precompiled output format.
  * inject precompiled files into a SWF by replacing or creating methods.

As3c works great with [Mono](http://www.mono-project.com/Main_Page) on Linux and OS X. You do not even have to recompile it.

## Syntax example ##
### Hello world ###
This example is equivalent to a simple `trace('Hello World!')`.

```
package  
{
	import flash.display.Sprite;
	
	import de.popforge.asm.Op;
	import de.popforge.asm.__asm;		


	/**
	 * Hello World example using As3c inline asm syntax.
	 *
	 * @author Joa Ebert
	 */
	public class Main extends Sprite 
	{
		public function Main()
		{
			__asm(
				Op.findPropertyStrict('public::trace'),
				Op.pushString('Hello World!'),
				Op.callPropertyVoid('public::trace', 1)´
			);
		}
	}
}
```

### New possibilities ###
When using asm you can define labels wherever you want. You can also jump to labels from any point in your code. This opens completly new possibilities as you can see. Although this one might be a little bit confusing ;)

```
package  
{
	import flash.display.Sprite;
	
	import de.popforge.asm.Op;
	import de.popforge.asm.__asm;		

	/**
	 * Asm example for new possibilities when using labels and inline asm.
	 * 
	 * @author Joa Ebert
	 */
	public class Main extends Sprite 
	{
		public function Main()
		{
			if (true === true)
			{
				__asm(Op.jump('.else'));
				__asm('.if:');
				
				trace('Hello if!');
			}
			else
			{
				__asm('.else:');
				
				trace('Hello else!');
				
				__asm(Op.jump('.if'));
			}
		}
	}
}
```

## Alternatives ##
If you are familiar with the [haXe](http://haxe.org/) language you might want to take a look at [hxASM](http://code.google.com/p/hxasm/). An ActionScript 2 alternative is [flasm](http://www.nowrap.de/flasm.html).