import * as React from 'react';
import TicketSummary from './TicketSummary';
import TicketProgress from './TicketProgress';
import TicketInformation from './TicketInformation';
import { Issue, CardColor } from './Interfaces';

const styleTicket = {
    background:''
}

const styleProgress = {
    background: ''
}

export default class Ticket extends React.Component<{ issue: Issue, colorList: CardColor[] }> {
    public render() {
        if (this.props.issue.key == '') {
            return null;
        }
        else {
            return <article style={Ticket.PriorityCheck(this.props.issue, this.props.colorList)}>
                <a href={"https://jira.returnonintelligence.com/browse/" + this.props.issue.key} target="_blank">
                    <TicketSummary summary={this.props.issue.fields.summary} />
                    <TicketInformation issue={this.props.issue} keyName={this.props.issue.key} />
                    <TicketProgress issue={this.props.issue} color={styleProgress.background} />
                </a>
            </article>
        }
    }

    private static PriorityCheck(issue: Issue, colorList: CardColor[]) {
        var Priority = issue.fields.priority.name;

        if (Priority == 'Blocker') {
            if (colorList.length > 0) {
                styleTicket.background = Ticket.ChangeColor(colorList[0].color);
                styleProgress.background = colorList[0].color;
            }
            else {
                styleTicket.background = '#FFE4D2';
                styleProgress.background = '#CC0000';
            }
        }
        else if (Priority == 'Critical') {
            if (colorList.length > 0) {
                styleTicket.background = Ticket.ChangeColor(colorList[1].color);
                styleProgress.background = colorList[1].color;
            }
            else {
                styleTicket.background = '#FFF0D9';
                styleProgress.background = '#FF9900';
            }
        }
        else if (Priority == 'Major') {
            if (colorList.length > 0) {
                styleTicket.background = Ticket.ChangeColor(colorList[2].color);
                styleProgress.background = colorList[2].color;
            }
            else {
                styleTicket.background = '#DDF1FF';
                styleProgress.background = '#0099FF';
            }
        }
        else if (Priority == 'Minor') {
            if (colorList.length > 0) {
                styleTicket.background = Ticket.ChangeColor(colorList[3].color);
                styleProgress.background = colorList[3].color;
            }
            else {
                styleTicket.background = '#DBFFCE';
                styleProgress.background = '#00CC00';
            }
        }
        else if (Priority == 'Trivial') {
            if (colorList.length > 0) {
                styleTicket.background = Ticket.ChangeColor(colorList[4].color);
                styleProgress.background = colorList[4].color;
            }
            else {
                styleTicket.background = '#D3D3D3';
                styleProgress.background = '#808080';
            }
        }

        return styleTicket;
    }

    private static ChangeColor(color: string) {

        var colorHex = color.slice(1);

        var r = parseInt(colorHex.substring(0, colorHex.length / 3), 16);
        var g = parseInt(colorHex.substring(colorHex.length / 3, 2 * colorHex.length / 3), 16);
        var b = parseInt(colorHex.substring(2 * colorHex.length / 3, 3 * colorHex.length / 3), 16);

        r /= 255, g /= 255, b /= 255;

        var max = Math.max(r, g, b);
        var min = Math.min(r, g, b);

        var h = (max + min) / 2;
        var s = (max + min) / 2;
        var l = (max + min) / 2;

        if (max == min) {
            h = s = 0;
        }
        else {
            var difference = max - min;

            s = l > 0.5 ? difference / (2 - max - min) : difference / (max + min);

            switch (max) {
                case r: h = 60 * ((g - b) / difference + (g < b ? 6 : 0)); break;
                case g: h = 60 * ((b - r) / difference + 2); break;
                case b: h = 60 * ((r - g) / difference + 4); break;
            }
        }
        
        if (l*100 > 80) {
            l = 100;
        }
        else {
            l = 90; 
        }
        return "hsl("+ h + "," + s*100 + "%," + l + "%)";
    }
}
