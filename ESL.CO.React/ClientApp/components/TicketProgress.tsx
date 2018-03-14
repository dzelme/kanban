import * as React from 'react';
import { Progress } from './Interfaces';

export default class TicketProgress extends React.Component<{ progress: Progress, created: Date }>{
    public render() {

        
       
        return <div>
            <div style={styleBar}><div style={TicketProgress.ProgressBar(this.props.progress.percent)}></div></div>

            <h3 style={styleProgress}>Date created: {this.props.created}</h3>
            <h3 style={styleProgress}>Difference: {TicketProgress.TimeCalculation(this.props.created)}</h3>
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
        var dayMinutes = differenceMinutes - differenceDays - differenceHours/* - (days * 24 * 60)*/;

        var hours = differenceHours;
        var hourMinutes = differenceMinutes - (hours * 60);
 

        if (years > 0) {

            if (years > 1) {
                queued = years + ' gadi ' + yearDays + ' dienas';
            }
            else {
                queued = years + ' gads ' + yearDays + ' dienas';
            }
        }
        else
        {
            queued = days + ' dienas ' + dayHours + ' stundas ' + dayMinutes + ' minutes';
        }

        return queued;
    }
}

const styleProgress = {
    paddingRight: '10px',
    paddingLeft:'10px'
}

const stylePercent = {
    width: '',
    height: '10px',
    background: 'blue',

}

const styleBar = {
    background: 'white',
    margin: '10px',
    height: '10px'
}