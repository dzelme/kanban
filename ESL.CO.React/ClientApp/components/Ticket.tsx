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

        var numberHex = parseInt(colorHex, 16);

        var r = (numberHex >> 16) + 125;
        if (r > 255) {
            r = 255;
        }
        else if (r < 50) {
            r = 50;
        }

        var g = (numberHex & 0x0000FF) + 125;
        if (g > 255) {
            g = 255;
        }
        else if (g < 50) {
            g = 50;
        }

        var b = ((numberHex >> 8) & 0x00FF) + 125;
        if (b > 255) {
            b = 255;
        }
        else if (b < 50) {
            b = 50;
        }

        return "#" + (g | (b << 8) | (r << 16)).toString(16);
    }
}
