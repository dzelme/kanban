import * as React from 'react';

export default class TicketAssignee extends React.Component<{ assigneeName: string }> {

    public render() {

        return <div style={style}>
            <h4>{this.props.assigneeName}</h4>
        </div>

    }
}
const style = {
    color: 'black',
};