using AudioManager.Locator;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandPlaySound : SequencerCommand
    {
        public void Awake()
        {
            string soundName = GetParameter(0);

            ServiceLocator.GetService().Play(soundName);

            Stop();
        }
    }
}