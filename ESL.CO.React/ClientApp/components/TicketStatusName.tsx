import * as React from 'react';

export default class TicketStatusName extends React.Component<{ statusName: string }> {
    public render() {

        return <div style={style}>
            <h4>{this.props.statusName}</h4>
        </div>

    }
}

const style = {
    color: 'black',
};