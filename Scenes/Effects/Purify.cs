using Godot;

namespace GWJ87.Scenes.Effects;

public partial class Purify : Node2D 
{
    private GpuParticles2D particles;

    public override void _Ready()
    {
        particles = GetNode<GpuParticles2D>("GPUParticles2D");
    }

    public void OnParticlesFinished()
    {
        SignalManager.Instance.EmitPurifyParticlesFinished();
    }

    public void EmitParticles()
    {
        particles.Emitting = false;
        particles.Restart();
        particles.Emitting = true;

        GD.Print("Particles Restarted");
    }
}
