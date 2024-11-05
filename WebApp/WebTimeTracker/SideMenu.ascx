<%@ Control Language="C#" AutoEventWireup="true" %>

<style>
    .sidebar {
        width: 200px;
        background-color: #000000;
        padding: 20px;
        height: 100vh;
        position: fixed;
        top: 0;
        left: 0;
        overflow-y: auto;
        border-right: 2px solid #4CAF50;
    }
    .sidebar h2 {
        font-size: 18px;
        text-align: center;
        color: white;
    }
    .sidebar a {
        display: block;
        padding: 10px;
        margin: 5px 0;
        color: #ffffff;
        text-decoration: none;
        background-color: #333;
        text-align: center;
        border-radius: 5px;
    }
    .sidebar a:hover {
        background-color: #444;
    }
</style>

<div class="sidebar">
    <a href="TimeEntry.aspx">Enter Time</a>
    <a href="EntriesByWeek.aspx?week=current">View Current Week</a>
    <a href="EntriesByWeek.aspx?week=previous">View Previous Week</a>
    <a href="EntriesByWeek.aspx?week=all">View Entire Project</a>
    <a href="PeerReviewEntry.aspx">Peer Review Entry</a>
    <a href="ViewPeerReview.aspx">View Peer Review Ratings</a>
</div>
