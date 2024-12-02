using Microsoft.Maui.Controls;

namespace Tracker
{
    public partial class AppTabbedPage : TabbedPage
    {
        public AppTabbedPage(int userId) // Pass utdId as a parameter
        {
            InitializeComponent();

            //  ClassList Tab
            Children.Add(new ClassList(userId)
            {
                Title = "Time Tracker",
                IconImageSource = "time_tracker_icon.png"
            });

            // PeerReviewPage
            Children.Add(new PRBuilder(userId) 
            {
                Title = "Peer Review",
                IconImageSource = "peer_review_icon.png"
            });

            // Add Review Editor
            Children.Add(new RatingsEditor(userId)
            {
                Title = "Edit Peer Review",
                IconImageSource = "edit_peer_review_icon.png"
            });
        }
    }
}
