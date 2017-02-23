using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace QueuServer.Media
{
    class MediaPlayer
    {
        public static void playNextTicket()
        {
            try
            {
                SoundPlayer simpleSound = new SoundPlayer(_Constants.SOUNDS_FOLDER + @"\ticket.wav");
                simpleSound.Play();
            }
            catch (Exception e)
            {

            }

        }
    }
}
