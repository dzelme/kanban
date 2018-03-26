import * as React from 'react';
import { Progress } from './Interfaces';

export default class TicketProgress extends React.Component<{ progress: Progress, created: Date }>{
    public render() {

        

        return <div style={styleSection}>
            <div style={styleBar}><div style={TicketProgress.ProgressBar(this.props.progress.percent)}></div></div>
            <h5>Queue: <strong>{TicketProgress.TimeCalculation(this.props.created)}</strong></h5>
        </div>
    }

    private static ProgressBar(progress: number) {

        stylePercent.width = progress + '%';

        return stylePercent;
    }

    private static TimeCalculation(createdTime: Date) {

        var queued;

        var today = new Date();

        var todayString = Date.parse(today.toString());
        var createdString = Date.parse(createdTime.toString());

        var differenceMS = todayString - createdString;

        var differenceYears = Math.floor((differenceMS) / (1000 * 60 * 60 * 24 * 365));
        var differenceDays = Math.floor((differenceMS) / (1000 * 60 * 60 * 24));
        var differenceHours = Math.floor((differenceMS) / (1000 * 60 * 60));
        var differenceMinutes = Math.floor((differenceMS) / (1000 * 60));

        var years = differenceYears; 
        var yearDays = differenceDays - (years * 365);

        var days = differenceDays;
        var dayHours = differenceHours - (days * 24);
        var dayMinutes = differenceMinutes - ((days * 24 * 60) + (dayHours * 60));

        if (years > 0) {

            if (years % 10 == 1 && years % 100 != 11)
            {
                if (yearDays % 10 == 1 && yearDays % 100 != 11) {
                    queued = years + ' gads ' + yearDays + ' diena';
                }
                else
                {
                    queued = years + ' gads ' + yearDays + ' dienas';
                }
            }
            else
            {
                if (yearDays % 10 == 1 && yearDays % 100 != 11) {
                    queued = years + ' gadi ' + yearDays + ' diena';
                }
                else {
                    queued = years + ' gadi ' + yearDays + ' dienas';
                }
            }
        }
        else
        {
            if (days % 10 == 1 && days % 100 != 11) {
                if (dayHours % 10 == 1 && dayHours % 100 != 11) {
                    if (dayMinutes % 10 == 1 && dayMinutes % 100 != 11) {
                        queued = days + ' diena ' + dayHours + ' stunda ' + dayMinutes + ' minūte';
                    }
                    else {
                        queued = days + ' diena ' + dayHours + ' stunda ' + dayMinutes + ' minūtes';
                    }
                }
                else {
                    if (dayMinutes % 10 == 1 && dayMinutes % 100 != 11) {
                        queued = days + ' diena ' + dayHours + ' stundas ' + dayMinutes + ' minūte';
                    }
                    else {
                        queued = days + ' diena ' + dayHours + ' stundas ' + dayMinutes + ' minūtes';
                    }
                }
            }
            else {
                if (dayHours % 10 == 1 && dayHours % 100 != 11) {
                    if (dayMinutes % 10 == 1 && dayMinutes % 100 != 11) {
                        queued = days + ' dienas ' + dayHours + ' stunda ' + dayMinutes + ' minūte';
                    }
                    else {
                        queued = days + ' dienas ' + dayHours + ' stunda ' + dayMinutes + ' minūtes';
                    }
                }
                else {
                    if (dayMinutes % 10 == 1 && dayMinutes % 100 != 11) {
                        queued = days + ' dienas ' + dayHours + ' stundas ' + dayMinutes + ' minūte';
                    }
                    else {
                        queued = days + ' dienas ' + dayHours + ' stundas ' + dayMinutes + ' minūtes';
                    }
                }
            }
        }

        return queued;
    }
}

const stylePercent = {
    width: '',
    height: '10px',
    background: 'blue'
}

const styleBar = {
    border: 'solid',
    borderWidth:'2px',
    borderColor: 'black',
    height: '14px'
}

const styleSection = {
    padding: '10px',
    background: 'white',
    color: 'black'
}