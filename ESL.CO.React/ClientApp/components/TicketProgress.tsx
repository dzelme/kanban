import * as React from 'react';
import { Issue } from './Interfaces';


const stylePercent = {
    position: 'absolute' as 'absolute',
    height: '16px',
    width: '',
    background: ''
}

const styleProgressBar = {
    position: 'relative' as 'relative',
    height: '16px',
    background: '#000'
}


export default class TicketProgress extends React.Component<{ issue: Issue }>{
    public render() {

        return <div className="Progress">
            <div className="ProgressBar" style={TicketProgress.Overdue(this.props.issue.fields.dueDate, this.props.issue.fields.resolutionDate, this.props.issue.fields.progress.percent)}>
                <div style={TicketProgress.ProgressBar(this.props.issue.fields.progress.percent, this.props.issue.fields.priority.name)}></div>
                <strong><p className="ProgressTime">{TicketProgress.RemainingTimeCalculation(this.props.issue.fields.timeTracking.remainingEstimateSeconds)}</p></strong>
            </div>
        </div>
    }

    private static ProgressBar(progress: number, priority: string) {

        stylePercent.width = progress + '%';

        if( priority == 'Blocker') {
            stylePercent.background = '#CC0000';
        }
        else if (priority == 'Critical') {
            stylePercent.background = '#FF9900';
        }
        else if (priority == 'Major') {
            stylePercent.background = '#0099FF';
        }
        else if (priority == 'Minor') {
            stylePercent.background = '#00CC00';
        }
        else if (priority == 'Trivial') {
            stylePercent.background = '#808080';
        }

        return stylePercent;
    }

    private static Overdue(dueDate: string, resolutionDate: string, progress: number) {

        if (dueDate != null && resolutionDate != null) {
            styleProgressBar.background = "#000"

        }
        else if (dueDate != null && resolutionDate == null) {
 
            var today = new Date();

            var todayString = Date.parse(today.toString());
            var dueDateString = Date.parse(dueDate);

            var differenceMS = todayString - dueDateString;

            if (differenceMS >= 0) {
                if (progress != 0) {
                    styleProgressBar.background = 'linear-gradient(to right,' + stylePercent.background + ', #FF0000)';
                }
                else {
                    styleProgressBar.background = "#FF0000"
                }
            }
            else {
                styleProgressBar.background = "#000"
            }
        }
        else {
            styleProgressBar.background = "#000"
        }

        return styleProgressBar;
    }

    private static RemainingTimeCalculation(timeSeconds: number) {
        const minute = 60;
        const hour = 3600;
        const day = 86400;
        var time = timeSeconds;
        var timeDays = 0;
        var timeHours = 0;
        var timeMinutes = 0;

        if (time >= day) {
            while (time >= day) {
                time -= day;
                timeDays++;
            }
        }

        if (time >= hour) {
            while (time >= hour) {
                time -= hour;
                timeHours++;
            }
        }

        if (time >= minute) {
            while (time >= minute) {
                time -= minute;
                timeMinutes++;
            }
        }

        if (timeSeconds == 0) {
            return null;
        }
        else if (timeDays > 0) {
            if (timeHours > 0) {
                return timeDays + "d " + timeHours + "h";
            }
            return timeDays + "d";
        }
        else if (timeHours > 0) {
            if (timeMinutes > 0) {
                return timeHours + "h " + timeMinutes + "m";
            }
            return timeHours + "h"
        }
        else {
            return timeMinutes + "m";
        }
    }
}