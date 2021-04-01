SinOsc foo => dac;
// set frequency from first arg
if( me.args() > 0 )
{
    me.arg(0) => Std.atof => foo.freq;
}
else
{
    444 => foo.freq;
}

// set gain from second arg
if( me.args() > 1 )
{
    me.arg(1) => Std.atof => foo.gain;
}

1::second => now;